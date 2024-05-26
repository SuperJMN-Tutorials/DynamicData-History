using System;
using System.Reactive.Linq;
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
    
    [Reactive]
    public TransactionStatus Status { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
}

public static class Mixin
{
    public static int GroupId(this TransactionModel model)
    {
        return model.ParentId ?? -model.Id;
    }
}