// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides methods to create observables that can pause and resume the source sequence at will.
/// </summary>
public static class ObservableExtensions
{
    /// <summary>
    /// When the pauser emits true, events are forwarded. When it emits false, events are suppressed.
    /// </summary>
    /// <param name="source">The source observable sequence.</param>
    /// <param name="pauser">The observable sequence used to pause and resume the source sequence.</param>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <returns>An observable sequence whose elements are the result of invoking the transform function on each element of source.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="pauser" /> is null.</exception>
    public static IObservable<TSource> Pausable<TSource>(this IObservable<TSource> source, IObservable<bool> pauser)
    {
        return pauser
            .DistinctUntilChanged()
            .Select(active => !active ? source : Observable.Empty<TSource>())
            .Switch();
    }

    /// <summary>
    /// A pausable operator that buffers the latest value.
    /// When paused, the source’s events are stored (only the most recent one is kept).
    /// When resumed, if there is a buffered value, it is immediately emitted.
    /// </summary>
    /// <param name="source">The source observable sequence.</param>
    /// <param name="pauser">The observable sequence used to pause and resume the source sequence.</param>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <returns>An observable sequence whose elements are the result of invoking the transform function on each element of source.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="pauser" /> is null.</exception>
    public static IObservable<TSource> PausableBuffered<TSource>(
        this IObservable<TSource> source,
        IObservable<bool> pauser)
    {
        return Observable.Create<TSource>(observer =>
        {
            var latestValue = default(TSource);
            var hasLatest = false;
            var isActive = true;

            // Subscribe to the pauser.
            var pauserSubscription = pauser
                .DistinctUntilChanged()
                .Subscribe(active =>
                {
                    isActive = !active;
                    // When becoming active, immediately emit the buffered value if one exists.
                    if (isActive && hasLatest)
                    {
                        observer.OnNext(latestValue);
                    }
                });

            // Subscribe to the source.
            var sourceSubscription = source.Subscribe(
                x =>
                {
                    latestValue = x;
                    hasLatest = true;
                    if (isActive)
                    {
                        observer.OnNext(x);
                    }
                },
                observer.OnError,
                observer.OnCompleted);

            return new CompositeDisposable(pauserSubscription, sourceSubscription);
        });
    }
}