using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class TransactionGroup : TransactionNode
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

        Amount = childrenChangeSet
            .TransformOnObservable(x => x.Amount)
            .ForAggregation()
            .Sum(i => i);
        
        status = hasAny.Select(b => b ? "Unconfirmed" : "Confirmed").ToProperty(this, x => x.Status).DisposeWith(disposables);
        amount = Amount.ToProperty(this, x => x.AmountProperty);
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => children;

    public sealed override IObservable<int> Amount { get; }

    public override string Status => status.Value;

    public override int AmountProperty => amount.Value;
}