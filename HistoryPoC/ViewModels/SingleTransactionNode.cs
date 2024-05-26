using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class SingleTransactionNode : TransactionNode
{
    private readonly ObservableAsPropertyHelper<TransactionStatus> status;

    public SingleTransactionNode(TransactionModel transactionModel)
    {
        Name = transactionModel.Name;
        Status = transactionModel.WhenAnyValue(x => x.Status);
        Date = transactionModel.WhenAnyValue(x => x.Date).Select(offset => new HumanizedDateTimeOffset(offset));
        Amount = Observable.Return(transactionModel.Amount);
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => new(new ObservableCollection<TransactionNode>());
    public sealed override IObservable<int> Amount { get; }
    public override IObservable<TransactionStatus> Status { get; }
    public override IObservable<HumanizedDateTimeOffset> Date { get; }
}