using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using HistoryPoC.Helpers;
using HistoryPoC.Model;
using HistoryPoC.ViewModels.History.Nodes;

namespace HistoryPoC.ViewModels.History;

public class TransactionHistoryViewModel : IDisposable
{
    public ReadOnlyObservableCollection<TransactionNode> Items { get; }

    private readonly CompositeDisposable disposable = new();

    public TransactionHistoryViewModel(ISourceCache<TransactionModel, int> sourceCache)
    {
        sourceCache
            .Connect()
            .Group(model => model.GroupId())
            .Transform(g => (TransactionNode)new TransactionGroupNode(g))
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