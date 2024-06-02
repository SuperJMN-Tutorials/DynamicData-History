using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DynamicData;
using HistoryPoC.Helpers;
using HistoryPoC.Model;
using HistoryPoC.ViewModels.History.Nodes;
using HistoryPoC.Views.History.Columns;

namespace HistoryPoC.ViewModels.History;

public class TreeDataGridTransactionHistoryViewModel : TransactionHistoryViewModel
{
    public TreeDataGridTransactionHistoryViewModel(ISourceCache<TransactionModel, int> sourceCache) : base(sourceCache)
    {
        var hierarchicalExpanderColumn = new HierarchicalExpanderColumn<TransactionNode>(new TextColumn<TransactionNode, string>("Name", x => x.Name), x => x.Children);

        var amountColumn = new TemplateColumn<TransactionNode>("Amount", new ObservableTemplate<TransactionNode, int>(x => x.Amount));
        var dateColumn = new TemplateColumn<TransactionNode>("Date", new MyTemplate<TransactionNode, TextBlock>(node => new TextBlock(), (node, block) =>
        {
            block.Bind(TextBlock.TextProperty, node.Date.Select(x => x.ToString()));
        }));
        
        var labelsColumn = new TemplateColumn<TransactionNode>("Labels", new MyTemplate<TransactionNode, LabelsView>(node => new LabelsView(), (node, view) =>
        {
            view.Bind(StyledElement.DataContextProperty, node.Labels);
        }));
        
        var hierarchicalTreeDataGridSource = new HierarchicalTreeDataGridSource<TransactionNode>(Items)
        {
            Columns =
            {
                hierarchicalExpanderColumn,
                dateColumn,
                amountColumn,
                labelsColumn
            },
        };

        Source = hierarchicalTreeDataGridSource;
    }

    public HierarchicalTreeDataGridSource<TransactionNode> Source { get; set; }
}