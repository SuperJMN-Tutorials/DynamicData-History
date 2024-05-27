using System;
using System.Collections.Generic;
using ReactiveUI.Fody.Helpers;

namespace HistoryPoC.ViewModels;

public class TransactionModel : ViewModelBase
{
    public TransactionModel(string name, int id, int? parentId)
    {
        Name = name;
        Id = id;
        ParentId = parentId;
    }

    public string Name { get; }
    
    public int Id { get; }
    
    public int? ParentId { get; set; }

    public int Amount { get; } = 10;
    
    public IEnumerable<string> Labels { get; } = ["Label", "Other label"];
    
    [Reactive]
    public TransactionStatus Status { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
    
    [Reactive]
    public bool IsConfirmed { get; set; }
    
    
}