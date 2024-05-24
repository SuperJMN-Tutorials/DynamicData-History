using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public abstract class TransactionItem : ViewModelBase
{
    private readonly ObservableAsPropertyHelper<int> amount;
    public ReadOnlyObservableCollection<TransactionItem> Children { get; protected set; }
    public string Name { get; set; }

    public TransactionItem(IObservable<int> observableAmount)
    {
        amount = observableAmount.ToProperty(this, x => x.Amount, scheduler: RxApp.MainThreadScheduler);
    }

    public int Amount => amount.Value;
    public abstract string Status { get; }
    public abstract int UnconfirmedCount { get; }
}