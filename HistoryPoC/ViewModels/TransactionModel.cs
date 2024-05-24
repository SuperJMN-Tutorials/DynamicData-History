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
    
    public int GroupId => ParentId ?? -Id;
    
    public IObservable<int> Amount { get; } = Observable.Return(10);
    
    [Reactive]
    public TransactionStatus Status { get; set; }
}