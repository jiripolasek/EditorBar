// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace JPSoftworks.EditorBar.Helpers;

internal readonly struct StopwatchStatement(string name) : IDisposable
{
    private readonly long _timestamp = Stopwatch.GetTimestamp();

    // static ctor that uses  [CallerMemberName] to get the name of the method
    public static StopwatchStatement Start([CallerMemberName] string name = "")
    {
        return new StopwatchStatement(name);
    }


    public void Dispose()
    {
#if DEBUG
        var endTimestamp = Stopwatch.GetTimestamp();
        var elapsedMilliseconds = (endTimestamp - this._timestamp) * 1000 / Stopwatch.Frequency;
        Debug.WriteLine($"[{name}] took {elapsedMilliseconds} ms");
#endif
    }
}