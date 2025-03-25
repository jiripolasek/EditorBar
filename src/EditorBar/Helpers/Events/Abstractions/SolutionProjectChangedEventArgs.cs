// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers.Events.Abstractions;

/// <summary>
/// Provides additional info when a solution or project change occurs.
/// </summary>
public sealed class SolutionProjectChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the reason for the solution or project change.
    /// </summary>
    public SolutionProjectChangeReason Reason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionProjectChangedEventArgs" /> class.
    /// </summary>
    /// <param name="reason">The reason for the solution or project change.</param>
    public SolutionProjectChangedEventArgs(SolutionProjectChangeReason reason)
    {
        this.Reason = reason;
    }
}