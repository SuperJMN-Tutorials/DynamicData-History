using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;
using HistoryPoC.ViewModels;
using ReactiveUI;

namespace HistoryPoC;

public class BindSelectionBehavior : Behavior<TreeDataGrid>
{
    public static readonly StyledProperty<Binding> SelectionProperty = AvaloniaProperty.Register<BindSelectionBehavior, Binding>(
        nameof(Selection));

    public Binding Selection
    {
        get => GetValue(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        this.WhenAnyValue(x => x.AssociatedObject.Source.Selection)
            .WhereNotNull()
            .Subscribe(model => ((MainViewModel)AssociatedObject.DataContext).Selection = (TreeDataGridRowSelectionModel<TransactionNode>) model);
    }
}