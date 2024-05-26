using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using Bogus;
using DynamicData;
using ReactiveUI;

namespace HistoryPoC.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public TreeDataGridRowSelectionModel<TransactionNode> Selection { get; set; }
    public MainViewModel()
    {
        var items = new TransactionModel[]
        {
            new TransactionModel("Single", 2, null){ Date = new DateTimeOffset(2024, 5, 12, 1, 5, 33, 2, TimeSpan.Zero)},
            new TransactionModel("Single", 1, null) { Date = new DateTimeOffset(2024, 4, 1, 12, 44, 2, TimeSpan.Zero)},
            new TransactionModel("Two", 4, 1){ Date = new DateTimeOffset(2024, 5, 1, 23, 50, 2, TimeSpan.Zero)},
            new TransactionModel("Three", 5, 1) { Date = new DateTimeOffset(2024, 5, 25, 22, 15, 2, TimeSpan.Zero)},
            new TransactionModel("Single", 3, null) { Date = new DateTimeOffset(2024, 5, 12, 1, 22, 21, 2, TimeSpan.Zero)},
        };
        var sourceCache = new SourceCache<TransactionModel, int>(x => x.Id);
        sourceCache.AddOrUpdate(items);

        sourceCache.PopulateFrom(Observable.Interval(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler).Select(n => new[]
        {
            new TransactionModel("New", 6 + (int)n, 1)
            {
                Date = DateTimeOffset.UtcNow
            }
        }));

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
            .Group(model => model.GroupId())
            .Transform(g => (TransactionNode)new TransactionGroup(g))
            .DisposeMany()
            .Bind(out ReadOnlyObservableCollection<TransactionNode> collection)
            .Subscribe();

        Items = collection;
    }

    public ReadOnlyObservableCollection<TransactionNode> Items { get; }
}