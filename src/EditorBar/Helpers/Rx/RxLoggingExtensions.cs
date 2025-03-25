// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reactive.Linq;
using Microsoft;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Extension methods for IObservable.
/// </summary>
internal static class RxLoggingExtensions
{
    /// <summary>
    /// Logs any onError notifications using the provided logger, then re-emits the error downstream.
    /// </summary>
    public static IObservable<T> LogOnError<T>(
        this IObservable<T> source,
        Action<Exception> logger)
    {
        Requires.NotNull(source, nameof(source));
        Requires.NotNull(logger, nameof(logger));

        return Observable.Create<T>(observer =>
        {
            return source.Subscribe(
                onNext: observer.OnNext,
                onError: ex =>
                {
                    logger(ex);
                    observer.OnError(ex);
                },
                onCompleted: observer.OnCompleted
            );
        });
    }

    /// <summary>
    /// Logs the error and retries the source observable indefinitely.
    /// </summary>
    public static IObservable<T> LogAndRetry<T>(
        this IObservable<T> source,
        Action<Exception> logger)
    {
        Requires.NotNull(source, nameof(source));
        Requires.NotNull(logger, nameof(logger));

        return source
            .Do(
                onNext: static _ => { },
                onError: logger,
                onCompleted: static () => { }
            )
            .Retry();
    }

    /// <summary>
    /// Logs the error and retries the source observable indefinitely.
    /// </summary>
    public static IObservable<T> LogAndRetry<T>(
        this IObservable<T> source,
        string? message = null)
    {
        Requires.NotNull(source, nameof(source));

        return source
            .Do(
                onNext: static _ => { },
                onError: ex => ex.Log(message ?? ""),
                onCompleted: static () => { }
            )
            .Retry();
    }
}