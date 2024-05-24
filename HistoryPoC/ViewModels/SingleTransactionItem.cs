using System.Reactive.Linq;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class SingleTransactionItem : TransactionItem
{
    private readonly ObservableAsPropertyHelper<string> status;

    public SingleTransactionItem(TransactionModel transactionModel) : base(transactionModel.Amount)
    {
        Name = transactionModel.Name;
        status = transactionModel.WhenAnyValue(x => x.Status).Select(x => x.ToString()).ToProperty(this, x => x.Status);
    }

    public override string Status => status.Value;

    public override int UnconfirmedCount => -1;
}