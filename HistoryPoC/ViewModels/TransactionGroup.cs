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
            .Transform(x => (TransactionNode)new SingleTransactionNode(x));

        childrenChangeSet
            .Bind(out children)
            .Subscribe()
            .DisposeWith(disposables);

        Name = group.Key.ToString();


        var sum = childrenChangeSet
            .TransformOnObservable(x => x.Amount)
            .Sum(i => i);

        Amount = sum;

        Date = childrenChangeSet
            .TransformOnObservable(x => x.Date.Select(y => y.Value.Ticks))
            .Maximum(i => i)
            .Select(i => new HumanizedDateTimeOffset(new DateTimeOffset(i, TimeSpan.Zero)));

        var childrenCount = childrenChangeSet.Count();

        var confirmedCount = childrenChangeSet
            .AutoRefreshOnObservable(x => x.IsConfirmed)
            .FilterOnObservable(x => x.IsConfirmed.Select(b => b == true))
            .Count()
            .CombineLatest(childrenCount, (confirmed, total) =>
            {
                if (confirmed == total)
                {
                    return true;
                }

                if (confirmed == 0)
                {
                    return false;
                }
                
                return (bool?)null;
            });

        IsConfirmed = confirmedCount;
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => children;

    public sealed override IObservable<int> Amount { get; }

    public override IObservable<TransactionStatus> Status { get; }

    public override IObservable<HumanizedDateTimeOffset> Date { get; }

    public override IObservable<bool?> IsConfirmed { get; }

    public void Dispose()
    {
        status.Dispose();
        disposables.Dispose();
        amount.Dispose();
    }
}