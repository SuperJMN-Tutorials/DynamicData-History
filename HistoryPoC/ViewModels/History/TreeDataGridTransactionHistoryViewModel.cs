﻿using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
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
        var blankColumn = new TemplateColumn<TransactionNode>("", new FuncDataTemplate<TransactionNode>((node, ns) => new TextBlock()));

        var groupIdColumn = new TemplateColumn<TransactionNode>("GroupId", new MyTemplate<TransactionNode, TextBlock>(node => new TextBlock() { VerticalAlignment = VerticalAlignment.Center }, (node, block) =>
        {
            block.Text = node is TransactionGroupNode g ? g.Key.ToString() : "";
        }));
        
        var amountColumn = new TemplateColumn<TransactionNode>("Amount", new MyTemplate<TransactionNode, TextBlock>(node => new TextBlock() { VerticalAlignment = VerticalAlignment.Center }, (node, block) =>
        {
            block.Bind(TextBlock.TextProperty, node.Amount.Select(x => x.ToString()));
        }));
        
        var dateColumn = new TemplateColumn<TransactionNode>("Date", new MyTemplate<TransactionNode, TextBlock>(node => new TextBlock() { VerticalAlignment = VerticalAlignment.Center }, (node, block) =>
        {
            block.Bind(TextBlock.TextProperty, node.Date.Select(x => x.ToString()));
        }));

        var labelsColumn = new TemplateColumn<TransactionNode>("Labels", new MyTemplate<TransactionNode, LabelsView>(node => new LabelsView()
        {
            VerticalAlignment = VerticalAlignment.Center
        }, (node, view) =>
        {
            view.Bind(StyledElement.DataContextProperty, node.Labels);
        }), width: GridLength.Star);
        
        var confirmationColumn = new TemplateColumn<TransactionNode>("Confirmed", new MyTemplate<TransactionNode, CheckBox>(node => new CheckBox()
        {
            VerticalAlignment = VerticalAlignment.Center
        }, (node, view) =>
        {
            view.Bind(ToggleButton.IsCheckedProperty, node.IsConfirmed);
        }));

        var expanderColumn = new HierarchicalExpanderColumn<TransactionNode>(blankColumn, x => x.Children, hasChildrenSelector: x => x.Children.Count > 1);

        var actions = new TemplateColumn<TransactionNode>("Actions", new FuncDataTemplate<TransactionNode>((node, ns) => new TextBlock()));

        var hierarchicalTreeDataGridSource = new HierarchicalTreeDataGridSource<TransactionNode>(Items)
        {
            Columns =
            {
                expanderColumn,
                groupIdColumn,
                confirmationColumn,
                amountColumn,
                dateColumn,
                labelsColumn,
                actions
            },
        };

        Source = hierarchicalTreeDataGridSource;
    }

    public HierarchicalTreeDataGridSource<TransactionNode> Source { get; set; }
}