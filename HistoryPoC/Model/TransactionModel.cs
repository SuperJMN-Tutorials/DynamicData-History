using System;
using System.Collections.Generic;
using HistoryPoC.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace HistoryPoC.Model;

public class TransactionModel : ViewModelBase
{
    public TransactionModel(int id, int? parentId)
    {
        Id = id;
        ParentId = parentId;
    }

    public int Id { get; }

    public int? ParentId { get; set; }

    public int Amount { get; } = 10;

    public IEnumerable<string> Labels { get; } = ["Label", "Other label"];

    public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;

    [Reactive]
    public bool IsConfirmed { get; set; }
}