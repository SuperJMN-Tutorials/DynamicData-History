using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class TransactionGroup : TransactionNode, IDisposable
{
    private readonly ObservableAsPropertyHelper<TransactionStatus> status;
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
            .Filter(x => x.Status == TransactionStatus.Unconfirmed, suppressEmptyChangeSets: false)
            .Count();


        var sum = childrenChangeSet
            .TransformOnObservable(x => x.Amount)
            .Sum(i => i);

        Amount = sum;

        Date = childrenChangeSet
            .TransformOnObservable(x => x.Date.Select(y => y.Ticks))
            .Maximum(i => i)
            .Select(i => new DateTimeOffset(i, TimeSpan.Zero));
        
        var hasAny =  unconfirmedCount
            .Select(n => n > 0);

        status = hasAny.Select(b => b ? TransactionStatus.Unconfirmed : TransactionStatus.Confirmed)
            .ToProperty(this, x => x.Status).DisposeWith(disposables);
        amount = Amount.ToProperty(this, x => x.AmountProperty);
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => children;

    public sealed override IObservable<int> Amount { get; }

    public override TransactionStatus Status => status.Value;

    public override int AmountProperty => amount.Value;

    public override IObservable<DateTimeOffset> Date { get; }

    public void Dispose()
    {
        status.Dispose();
        disposables.Dispose();
        amount.Dispose();
    }
}