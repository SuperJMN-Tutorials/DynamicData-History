using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;

namespace HistoryPoC.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public MainViewModel()
    {
        var items = new TransactionModel[]
        {
            new TransactionModel("Single", 1, null),
            new TransactionModel("Single", 2, null),
            new TransactionModel("Single", 3, null),
            new TransactionModel("Two", 4, 1),
            new TransactionModel("Three", 5, 1)
        };

        var sourceCache = new SourceCache<TransactionModel, int>(x => x.Id);
        sourceCache.AddOrUpdate(items);

        sourceCache.PopulateFrom(Observable.Interval(TimeSpan.FromSeconds(5)).Select(n => new[] { new TransactionModel("New", 6 + (int)n, 1) }));
        

        sourceCache
            .Connect()
            .Group(model => model.GroupId)
            .Transform(g => (TransactionItem) new TransactionGroup(g))
            .DisposeMany()
            .Bind(out ReadOnlyObservableCollection<TransactionItem> collection)
            .Subscribe();

        Items = collection;
    }

    public ReadOnlyObservableCollection<TransactionItem> Items { get; }
}

public abstract class TransactionItem
{
    public ReadOnlyObservableCollection<TransactionItem> Children { get; protected set; }
    public string Name { get; set; }
}

public class TransactionGroup : TransactionItem
{
    public TransactionGroup(IGroup<TransactionModel, int, int> group)
    {
        group.Cache.Connect()
            .Transform(x => (TransactionItem)new SingleTransactionItem(x))
            .Bind(out var children)
            .Subscribe();
        Children = children;
        Name = group.Key.ToString();
    }
}

public class SingleTransactionItem : TransactionItem
{
    public SingleTransactionItem(TransactionModel transactionModel) : base()
    {
        Name = transactionModel.Name;
    }
}

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
}
