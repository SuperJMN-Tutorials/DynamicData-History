using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace HistoryPoC.Helpers;

public class ObservableTemplate<T, TOutput> : IDataTemplate
{
    private readonly Func<T, IObservable<TOutput>> _build;

    public ObservableTemplate(Func<T, IObservable<TOutput>> build)
    {
        _build = build;
    }

    public Control Build(object param)
    {
        var binding = _build((T)param).ToBinding();

        return new ContentPresenter()
        {
            Margin = new Thickness(4),
            [!ContentControl.ContentProperty] = binding,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };
    }

    public bool Match(object data)
    {
        return data is T;
    }
}