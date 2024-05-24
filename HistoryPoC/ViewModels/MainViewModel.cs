using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using Bogus;
using DynamicData;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public MainViewModel()
    {
        var items = new TransactionModel[]
        {
            new TransactionModel("Single", 1, null),
            new TransactionModel("Single", 2, null),
            new TransactionModel("Single", 3, null),
            new TransactionModel("Two", 4, 1),
            new TransactionModel("Three", 5, 1)
        };

        var sourceCache = new SourceCache<TransactionModel, int>(x => x.Id);
        sourceCache.AddOrUpdate(items);

        //sourceCache.PopulateFrom(Observable.Interval(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler).Select(n => new[] { new TransactionModel("New", 6 + (int)n, 1) }));

        var faker = new Faker();

        Observable.Interval(TimeSpan.FromSeconds(3), RxApp.MainThreadScheduler)
            .Subscribe(l =>
            {
                var randomItem = faker.PickRandom(sourceCache.Items);
                var value = faker.PickRandom(Enum.GetValues<TransactionStatus>());
                //randomItem.Status = faker.PickRandom(value);
                randomItem.Status = TransactionStatus.Unconfirmed;
            });

        sourceCache
            .Connect()
            .Group(model => model.GroupId)
            .Transform(g => (TransactionItem)new TransactionGroup(g))
            .DisposeMany()
            .Bind(out ReadOnlyObservableCollection<TransactionItem> collection)
            .Subscribe();

        Items = collection;
    }

    public ReadOnlyObservableCollection<TransactionItem> Items { get; }
}