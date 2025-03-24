// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace JPSoftworks.EditorBar.Helpers;

public static class SelectNothingObservableExtensions
{
    public static IObservable<Unit> SelectToLast<T>(this IObservable<T> source, Func<T, Task> selector)
    {
        return source
            .Select(t => Observable.FromAsync(_ => selector(t)))
            .Switch();
    }

    public static IObservable<TResult> SelectToLast<T, TResult>(
        this IObservable<T> source,
        Func<T, Task<TResult>> selector)
    {
        return source
            .Select(t => Observable.FromAsync(_ => selector(t)))
            .Switch();
    }

    public static IObservable<Unit> SelectToLast<T>(
        this IObservable<T> source,
        Func<T, CancellationToken, Task> selector)
    {
        return source
            .Select(t => Observable.FromAsync(ct => selector(t, ct)))
            .Switch();
    }

    public static IObservable<TResult> SelectToLast<T, TResult>(
        this IObservable<T> source,
        Func<T, CancellationToken, Task<TResult>> selector)
    {
        return source
            .Select(t => Observable.FromAsync(ct => selector(t, ct)))
            .Switch();
    }
}