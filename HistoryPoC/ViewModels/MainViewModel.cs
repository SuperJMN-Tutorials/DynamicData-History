using System;
using System.Reactive.Disposables;
using DynamicData;
using HistoryPoC.Helpers;
using HistoryPoC.Model;
using HistoryPoC.ViewModels.History;

namespace HistoryPoC.ViewModels;

public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly CompositeDisposable disposable =new();

    public MainViewModel()
    {
        var items = new TransactionModel[]
        {
            new TransactionModel(1, null) { Date = new DateTimeOffset(2024, 4, 1, 12, 44, 2, TimeSpan.Zero)},
            new TransactionModel(2, null){ Date = new DateTimeOffset(2024, 5, 12, 1, 5, 33, 2, TimeSpan.Zero)},
            new TransactionModel(3, null) { Date = new DateTimeOffset(2024, 5, 12, 1, 22, 21, 2, TimeSpan.Zero)},
            new TransactionModel(4, 1){ Date = new DateTimeOffset(2024, 5, 1, 23, 50, 2, TimeSpan.Zero)},
            new TransactionModel(5, 1) { Date = new DateTimeOffset(2024, 5, 25, 22, 15, 2, TimeSpan.Zero)},
        };

        var sourceCache = new SourceCache<TransactionModel, int>(x => x.Id);
        sourceCache.AddOrUpdate(items);
        
        HistoryViewModel = new TransactionHistoryViewModel(sourceCache);
        TreeDataGridHistoryViewModel = new TreeDataGridTransactionHistoryViewModel(sourceCache);
        new Mutator().Mutate(sourceCache)
            .DisposeWith(disposable);
    }

    public TreeDataGridTransactionHistoryViewModel TreeDataGridHistoryViewModel { get; }

    public TransactionHistoryViewModel HistoryViewModel { get; }

    public void Dispose()
    {
        disposable.Dispose();
        TreeDataGridHistoryViewModel.Dispose();
        HistoryViewModel.Dispose();
    }
}