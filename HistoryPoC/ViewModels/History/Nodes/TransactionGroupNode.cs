using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Aggregation;
using HistoryPoC.Helpers;
using HistoryPoC.Model;

namespace HistoryPoC.ViewModels.History.Nodes;

public class TransactionGroupNode : TransactionNode, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly ReadOnlyObservableCollection<TransactionNode> children;

    public TransactionGroupNode(IGroup<TransactionModel, int, int> group)
    {
        Key = group.Key;
        
        var changeSet = group.Cache.Connect();

        var childrenChangeSet = changeSet
            .Transform(x => (TransactionNode)new SingleTransactionNode(x));

        childrenChangeSet
            .Bind(out children)
            .Subscribe()
            .DisposeWith(disposables);

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
        Labels = new BehaviorSubject<IEnumerable<string>>(new List<string>(){"Sample", "Value"});
    }

    public int Key { get; set; }

    public override ReadOnlyObservableCollection<TransactionNode> Children => children;

    public sealed override IObservable<int> Amount { get; }

    public override IObservable<HumanizedDateTimeOffset> Date { get; }
    public override IObservable<bool?> IsConfirmed { get; }
    public override IObservable<IEnumerable<string>> Labels { get; }

    public void Dispose()
    {
        disposables.Dispose();
    }
}