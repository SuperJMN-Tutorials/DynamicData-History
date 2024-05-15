using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace HistoryPoC.Views;

public partial class MainView : UserControl
{
    private ObservableCollection<Model> models = new ObservableCollection<Model>()
    {
        new Model()
        {
            Name = "Hola",
            Children = new ObservableCollection<Model>()
            {
                new Model()
                {
                    Name="Adiós",
                    Children = new ObservableCollection<Model>()
                }
            }
        }
    };

    public MainView()
    {
        Source = new HierarchicalTreeDataGridSource<Model>(models)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<Model>(new TextColumn<Model, string>("Name", model => model.Name), x => x.Children)
            }
        };
        
        InitializeComponent();
    }

    public HierarchicalTreeDataGridSource<Model> Source { get; set; }
}

public class Model
{
    public ObservableCollection<Model> Children { get; set; } = new ObservableCollection<Model>();
    public string? Name { get; set; }
}
