﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Bogus;
using DynamicData;
using HistoryPoC.Model;
using ReactiveUI;

namespace HistoryPoC.Helpers;

public class Mutator
{
    public IDisposable Mutate(ISourceCache<TransactionModel, int> sourceCache)
    {
        var disposable = new CompositeDisposable();

        var faker = new Faker();

        Observable.Interval(TimeSpan.FromSeconds(3), RxApp.MainThreadScheduler)
            .Subscribe(l =>
            {
                var randomItem = faker.PickRandom(sourceCache.Items);
                //randomItem.Status = faker.PickRandom(value);
                randomItem.IsConfirmed = true;
            })
            .DisposeWith(disposable);

        sourceCache.PopulateFrom(Observable.Interval(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler).Select(n => new[]
            {
                new TransactionModel(Random.Shared.Next(), 1)
                {
                    Date = DateTimeOffset.UtcNow
                }
            }))
            .DisposeWith(disposable);


        return disposable;
    }
}