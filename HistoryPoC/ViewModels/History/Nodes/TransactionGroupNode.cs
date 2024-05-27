﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using HistoryPoC.Helpers;
using HistoryPoC.Model;
using ReactiveUI;

namespace HistoryPoC.ViewModels.History.Nodes;

public class TransactionGroupNode : TransactionNode, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly ReadOnlyObservableCollection<TransactionNode> children;
    private readonly ObservableAsPropertyHelper<int> amount;

    public TransactionGroupNode(IGroup<TransactionModel, int, int> group)
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
        Labels = Observable.Return(new List<string>() { "Sample", "Label" });
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => children;

    public sealed override IObservable<int> Amount { get; }

    public override IObservable<HumanizedDateTimeOffset> Date { get; }
    public override IObservable<bool?> IsConfirmed { get; }
    public override IObservable<IEnumerable<string>> Labels { get; }

    public void Dispose()
    {
        disposables.Dispose();
        amount.Dispose();
    }
}