using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;

namespace HistoryPoC.ViewModels;

public class TransactionHistoryViewModel : IDisposable
{
    public ReadOnlyObservableCollection<TransactionNode> Items { get; }

    private readonly CompositeDisposable disposable = new();
    
    public TransactionHistoryViewModel(ISourceCache<TransactionModel, int> sourceCache)
    {
        sourceCache
            .Connect()
            .Group(model => model.GroupId())
            .Transform(g => (TransactionNode)new TransactionGroup(g))
            .DisposeMany()
            .Bind(out var items)
            .Subscribe()
            .DisposeWith(disposable);
        
        Items = items;
    }

    public void Dispose()
    {
        disposable.Dispose();
    }
}