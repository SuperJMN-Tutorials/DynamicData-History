using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Data.Converters;
using HistoryPoC.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace HistoryPoC.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    [Reactive]
    public HierarchicalTreeDataGridSource<TransactionNode> Source { get; private set; }

    public static readonly FuncValueConverter<IEnumerable, HierarchicalTreeDataGridSource<TransactionNode>> SourceConverter = new(items => CreateSource(items?.Cast<TransactionNode>() ?? new List<TransactionNode>()));

    private static HierarchicalTreeDataGridSource<TransactionNode> CreateSource(IEnumerable<TransactionNode> groups)
    {
        var hierarchicalTreeDataGridSource = new HierarchicalTreeDataGridSource<TransactionNode>(groups)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<TransactionNode>(new TextColumn<TransactionNode,string>("Name", x => x.Name), x => x.Children),
                new TextColumn<TransactionNode,int>("Amount", x => x.AmountProperty),
                new TextColumn<TransactionNode, string>("Status", x => x.Status),
            },
        };

        return hierarchicalTreeDataGridSource;
    }
}