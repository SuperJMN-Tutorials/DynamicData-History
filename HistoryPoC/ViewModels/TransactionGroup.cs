using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class TransactionGroup : TransactionItem
{
    private readonly ObservableAsPropertyHelper<string> status;
    private readonly ObservableAsPropertyHelper<int> unconfirmed;
    private readonly CompositeDisposable disposables = new();

    public TransactionGroup(IGroup<TransactionModel, int, int> group) : base(group.Cache.Connect()
        .TransformOnObservable(x => x.Amount)
        .ForAggregation()
        .Sum(i => i))
    {
        var changeSet = group.Cache.Connect();

        changeSet
            .Transform(x => (TransactionItem) new SingleTransactionItem(x))
            .Bind(out var children)
            .Subscribe()
            .DisposeWith(disposables);
        
        Children = children;
        Name = group.Key.ToString();

        var unconfirmedCount = changeSet
            .AutoRefresh()
            .Filter(x => x.Status == TransactionStatus.Unconfirmed, suppressEmptyChangeSets: false)
            .Count();

        unconfirmed = unconfirmedCount.ToProperty(this, x => x.UnconfirmedCount).DisposeWith(disposables);
        
        IObservable<bool> hasAny =  unconfirmedCount
            .Select(n => n > 0);
        
        status = hasAny.Select(b => b ? "Unconfirmed" : "Confirmed").ToProperty(this, x => x.Status).DisposeWith(disposables);
    }

    public override int UnconfirmedCount => unconfirmed.Value;

    public override string Status => status.Value;
}