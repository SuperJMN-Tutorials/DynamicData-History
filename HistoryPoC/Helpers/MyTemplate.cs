using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace HistoryPoC.Helpers;

public class MyTemplate<TNode, TControl> : IDataTemplate where TControl : Control
{
    public MyTemplate(Func<TNode?, TControl> controlFactory, Action<TNode, TControl> bind)
    {
        Bind = bind;
        ControlFactory = controlFactory;
    }

    public Control? Build(object? param)
    {
        if (param is null)
        {
            return null;
        }

        var node = (TNode) param;
        var control = ControlFactory(node);
        Bind(node, control);
        return control;
    }

    private Action<TNode, TControl> Bind { get; }

    private Func<TNode?, TControl> ControlFactory {get;}

    public bool Match(object? data) => data is TNode;
}