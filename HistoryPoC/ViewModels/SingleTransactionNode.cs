using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class SingleTransactionNode : TransactionNode
{
    public SingleTransactionNode(TransactionModel transactionModel)
    {
        Name = transactionModel.Name;
        Date = transactionModel.WhenAnyValue(x => x.Date).Select(offset => new HumanizedDateTimeOffset(offset));
        Amount = Observable.Return(transactionModel.Amount);
        IsConfirmed = transactionModel.WhenAnyValue(x => x.IsConfirmed, b =>(bool?)b);
        Labels = Observable.Return(transactionModel.Labels);
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => new(new ObservableCollection<TransactionNode>());
    public sealed override IObservable<int> Amount { get; }
    public override IObservable<HumanizedDateTimeOffset> Date { get; }
    public override IObservable<bool?> IsConfirmed { get; }
    public override IObservable<IEnumerable<string>> Labels { get; }
}