﻿using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public abstract class TransactionNode : ViewModelBase
{
    public abstract ReadOnlyObservableCollection<TransactionNode> Children { get; }
    public string Name { get; set; }
    public abstract IObservable<int> Amount { get; }
    public abstract string Status { get; }
    public abstract int AmountProperty { get; }
}