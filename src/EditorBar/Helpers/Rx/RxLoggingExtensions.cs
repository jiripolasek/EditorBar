// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
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
        _ = Requires.NotNull(source);
        _ = Requires.NotNull(logger);

        return Observable.Create<T>(observer =>
        {
            return source.Subscribe(
                observer.OnNext,
                ex =>
                {
                    logger(ex);
                    observer.OnError(ex);
                },
                observer.OnCompleted);
        });
    }

    /// <summary>
    /// Logs the error and retries the source observable indefinitely.
    /// </summary>
    public static IObservable<T> LogAndRetry<T>(
        this IObservable<T> source,
        Action<Exception> logger)
    {
        _ = Requires.NotNull(source);
        _ = Requires.NotNull(logger);

        return source
            .Do(
                static _ => { },
                logger,
                static () => { })
            .Retry();
    }

    /// <summary>
    /// Logs the error and retries the source observable indefinitely.
    /// </summary>
    public static IObservable<T> LogAndRetry<T>(
        this IObservable<T> source,
        string? message = null)
    {
        _ = Requires.NotNull(source);

        return source
            .Do(
                static _ => { },
                ex => ex.Log(message ?? string.Empty),
                static () => { })
            .Retry();
    }
}
