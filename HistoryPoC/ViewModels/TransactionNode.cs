using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HistoryPoC.ViewModels;

public abstract class TransactionNode : ViewModelBase
{
    public abstract ReadOnlyObservableCollection<TransactionNode> Children { get; }
    public string Name { get; set; }
    public abstract IObservable<int> Amount { get; }
    public abstract IObservable<HumanizedDateTimeOffset> Date { get; }
    public abstract IObservable<bool?> IsConfirmed { get; }
    public abstract IObservable<IEnumerable<string>> Labels { get; }
}