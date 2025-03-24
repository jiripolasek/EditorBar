// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Creates instances of RX.NET subjects using the factory method pattern.
/// </summary>
internal static class SubjectFactory
{
    // Factory method that creates a BehaviorSubject<T> using the provided initial observable.
    public static BehaviorSubject<T> Create<T>(T initialValue)
    {
        return new BehaviorSubject<T>(initialValue);
    }

    // Optional overload: creates a BehaviorSubject<T> with an empty observable as its initial value.
    public static BehaviorSubject<IObservable<T>> CreateEmpty<T>()
    {
        return new BehaviorSubject<IObservable<T>>(Observable.Empty<T>());
    }
}