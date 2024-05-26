using System;
using System.Collections.ObjectModel;
using Humanizer;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public abstract class TransactionNode : ViewModelBase
{
    public abstract ReadOnlyObservableCollection<TransactionNode> Children { get; }
    public string Name { get; set; }
    public abstract IObservable<int> Amount { get; }
    public abstract IObservable<TransactionStatus> Status { get; }
    public abstract IObservable<HumanizedDateTimeOffset> Date { get; }
}

public record HumanizedDateTimeOffset
{
    public DateTimeOffset Value { get; }

    public HumanizedDateTimeOffset(DateTimeOffset value)
    {
        Value = value;
    }

    public override string ToString() => Value.Humanize(DateTimeOffset.Now);
}