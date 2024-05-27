using System;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DynamicData;
using HistoryPoC.Helpers;
using HistoryPoC.Model;
using HistoryPoC.ViewModels.History.Nodes;

namespace HistoryPoC.ViewModels.History;

public class TreeDataGridTransactionHistoryViewModel : TransactionHistoryViewModel
{
    public TreeDataGridTransactionHistoryViewModel(ISourceCache<TransactionModel, int> sourceCache) : base(sourceCache)
    {
        var hierarchicalExpanderColumn = new HierarchicalExpanderColumn<TransactionNode>(new TextColumn<TransactionNode, string>("Name", x => x.Name), x => x.Children);

        var amountColumn = new TemplateColumn<TransactionNode>("Amount", new ObservableTemplate<TransactionNode, int>(x => x.Amount));
        var dateColumn = new TemplateColumn<TransactionNode>("Date", new ObservableTemplate<TransactionNode, HumanizedDateTimeOffset>(x => x.Date));
        
        var hierarchicalTreeDataGridSource = new HierarchicalTreeDataGridSource<TransactionNode>(Items)
        {
            Columns =
            {
                hierarchicalExpanderColumn,
                dateColumn,
                amountColumn
            },
        };

        Source = hierarchicalTreeDataGridSource;
    }

    public HierarchicalTreeDataGridSource<TransactionNode> Source { get; set; }
}