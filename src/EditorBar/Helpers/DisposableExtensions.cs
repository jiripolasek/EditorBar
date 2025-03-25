// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft;

// ReSharper disable once CheckNamespace
namespace System.Reactive.Disposables;

/// <summary>
/// Extension methods for <see cref="IDisposable" />.
/// </summary>
public static class DisposableExtensions
{
    /// <summary>
    /// Adds a disposable object to a composite collection for management. Returns the original disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">Represents a type that implements a mechanism for releasing resources.</typeparam>
    /// <param name="disposable">The object that will be added to the composite collection for resource management.</param>
    /// <param name="composite">The collection that manages multiple disposable objects for coordinated disposal.</param>
    /// <exception cref="ArgumentNullException"><paramref name="disposable" /> or <paramref name="composite" /> is <see langword="null" />.</exception>
    /// <returns>The original disposable object that was added to the composite.</returns>
    public static TDisposable AddTo<TDisposable>(this TDisposable disposable, CompositeDisposable composite)
        where TDisposable : IDisposable
    {
        Requires.NotNullAllowStructs(disposable, nameof(disposable));
        Requires.NotNull(composite, nameof(composite));

        composite.Add(disposable);
        return disposable;
    }
}