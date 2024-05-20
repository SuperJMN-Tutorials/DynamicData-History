using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using HistoryPoC.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace HistoryPoC.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        
        this.WhenAnyValue(x => x.DataContext)
            .WhereNotNull()
            .Select(x => CreateSource(((MainViewModel)x).Items))
            .Subscribe(x =>
            {
                TreeDataGrid.Source = x;
            });
    }

    [Reactive]
    public HierarchicalTreeDataGridSource<TransactionItem> Source { get; private set; }

    private HierarchicalTreeDataGridSource<TransactionItem> CreateSource(ReadOnlyObservableCollection<TransactionItem> groups)
    {
        return new HierarchicalTreeDataGridSource<TransactionItem>(groups)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<TransactionItem>(new TextColumn<TransactionItem,string>("Name", x => x.Name), x => x.Children)
            }
        };
    }
}