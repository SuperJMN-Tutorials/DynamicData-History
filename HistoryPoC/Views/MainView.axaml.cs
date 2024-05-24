using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Data;
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
        var hierarchicalTreeDataGridSource = new HierarchicalTreeDataGridSource<TransactionItem>(groups)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<TransactionItem>(new TextColumn<TransactionItem,string>("Name", x => x.Name), x => x.Children),
                new TextColumn<TransactionItem,int>("Amount", x => x.Amount),
                new TextColumn<TransactionItem, string>("Status", x => x.Status),
            },
        };

        var selection = new TreeDataGridRowSelectionModel<TransactionItem>(hierarchicalTreeDataGridSource);

        hierarchicalTreeDataGridSource.Selection = selection;
        selection.SelectionChanged += (sender, args) => { };

        return hierarchicalTreeDataGridSource;
    }
}