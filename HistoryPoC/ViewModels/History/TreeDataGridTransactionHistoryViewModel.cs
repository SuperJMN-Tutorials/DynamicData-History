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
        var hierarchicalTreeDataGridSource = new HierarchicalTreeDataGridSource<TransactionNode>(Items)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<TransactionNode>(new TextColumn<TransactionNode, string>("Name", x => x.Name), x => x.Children),
                new TemplateColumn<TransactionNode>("Amount", new ObservableTemplate<TransactionNode, int>(x => x.Amount))
            },
        };

        Source = hierarchicalTreeDataGridSource;
    }

    public HierarchicalTreeDataGridSource<TransactionNode> Source { get; set; }
}