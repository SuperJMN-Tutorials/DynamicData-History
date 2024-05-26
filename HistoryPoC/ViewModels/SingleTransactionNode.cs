using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class SingleTransactionNode : TransactionNode
{
    private readonly ObservableAsPropertyHelper<string> status;
    private readonly ObservableAsPropertyHelper<int> amount;

    public SingleTransactionNode(TransactionModel transactionModel)
    {
        Name = transactionModel.Name;
        status = transactionModel.WhenAnyValue(x => x.Status).Select(x => x.ToString()).ToProperty(this, x => x.Status);
        Amount = Observable.Return(transactionModel.Amount);
        amount = Amount.ToProperty(this, x => x.AmountProperty);
    }

    public override ReadOnlyObservableCollection<TransactionNode> Children => new(new ObservableCollection<TransactionNode>());
    public sealed override IObservable<int> Amount { get; }
    public override string Status => status.Value;
    public override int AmountProperty => amount.Value;

    public override IObservable<DateTimeOffset> Date => Observable.Return(DateTimeOffset.Now);
}