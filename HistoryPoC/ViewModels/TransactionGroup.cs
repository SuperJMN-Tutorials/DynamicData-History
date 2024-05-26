using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class TransactionGroup : TransactionNode, IDisposable
{
    private readonly ObservableAsPropertyHelper<string> status;
    private readonly CompositeDisposable disposables = new();
    private readonly ReadOnlyObservableCollection<TransactionNode> children;
    private readonly ObservableAsPropertyHelper<int> amount;

    public TransactionGroup(IGroup<TransactionModel, int, int> group)
    {
        var changeSet = group.Cache.Connect();

        var childrenChangeSet = changeSet
            .Transform(x => (TransactionNode) new SingleTransactionNode(x));
        
        childrenChangeSet
            .Bind(out children)
            .Subscribe()
            .DisposeWith(disposables);
        
        Name = group.Key.ToString();

        var unconfirmedCount = childrenChangeSet
            .AutoRefresh()
            .Filter(x => x.Status == "Unconfirmed", suppressEmptyChangeSets: false)
            .Count();

        var hasAny =  unconfirmedCount
            .Select(n => n > 0);

        var sum = childrenChangeSet
            .TransformOnObservable(x => x.Amount)
            .ForAggregation()
            .Sum(i => i);
        
        Amount = sum;

        var forAggregation = childrenChangeSet
            .TransformOnObservable(x => x.Date.Select(y => (long) y.Ticks))
            .ForAggregation()
            .Maximum(i => i)
            .Select(i => new DateTimeOffset(i, TimeSpan.Zero));
        
        Date = forAggregation;
        
        status = hasAny.Select(b => b ? "Unconfirmed" : "Confirmed").ToProperty(this, x => x.Status).DisposeWith(disposables);
        amount = Amount.ToProperty(this, x => x.AmountProperty);
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => children;

    public sealed override IObservable<int> Amount { get; }

    public override string Status => status.Value;

    public override int AmountProperty => amount.Value;

    public override IObservable<DateTimeOffset> Date { get; }

    public void Dispose()
    {
        status.Dispose();
        disposables.Dispose();
        amount.Dispose();
    }
}

public static class MyMixin
{
    /// <summary>
    /// Continual computes the sum of values matching the value selector.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="valueSelector">The value selector.</param>
    /// <returns>An observable which emits the summed value.</returns>
    public static IObservable<int> Sum<T>(this IObservable<IAggregateChangeSet<T>> source, Func<T, int> valueSelector)
    {
        return source.Accumulate(0, valueSelector, (current, value) => current + value, (current, value) => current - value);
    }
    
    public static IObservable<long> Maximum<T>(this IObservable<IAggregateChangeSet<T>> source, Func<T, long> valueSelector)
    {
        return source.Accumulate(long.MinValue, valueSelector, (i, i1) => Math.Max(i, i1), (i, i1) => Math.Min(i, i1));
    }
    
    internal static IObservable<TResult> Accumulate<TObject, TKey, TResult>(this IObservable<IChangeSet<TObject, TKey>> source, TResult seed, Func<TObject, TResult> accessor, Func<TResult, TResult, TResult> addAction, Func<TResult, TResult, TResult> removeAction)
        where TObject : notnull
        where TKey : notnull => source.ForAggregation().Accumulate(seed, accessor, addAction, removeAction);

    internal static IObservable<TResult> Accumulate<TObject, TResult>(this IObservable<IChangeSet<TObject>> source, TResult seed, Func<TObject, TResult> accessor, Func<TResult, TResult, TResult> addAction, Func<TResult, TResult, TResult> removeAction)
        where TObject : notnull => source.ForAggregation().Accumulate(seed, accessor, addAction, removeAction);

    internal static IObservable<TResult> Accumulate<TObject, TResult>(this IObservable<IAggregateChangeSet<TObject>> source, TResult seed, Func<TObject, TResult> accessor, Func<TResult, TResult, TResult> addAction, Func<TResult, TResult, TResult> removeAction)
    {

        return source.Scan(seed, (state, changes) =>
            changes.Aggregate(state, (current, aggregateItem) =>
                aggregateItem.Type == AggregateType.Add ? addAction(current, accessor(aggregateItem.Item)) : removeAction(current, accessor(aggregateItem.Item))));
    }

}