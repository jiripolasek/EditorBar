// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft;

namespace JPSoftworks.EditorBar.Helpers;

internal class SingleActionGatedExecutor
{
    private readonly Action _action; // The single action, set via constructor
    private readonly object _lockObject = new();
    private bool _executionPending; // Tracks if an execution request is pending
    private bool _gateOpen; // Tracks whether gate is open or closed

    public SingleActionGatedExecutor(Action action)
    {
        Requires.NotNull(action, nameof(action));

        this._action = action;
    }

    /// <summary>
    /// Opens the gate. If an execution request was pending, it executes now.
    /// </summary>
    public void OpenGate()
    {
        lock (this._lockObject)
        {
            this._gateOpen = true;
            if (this._executionPending)
            {
                this._action();
                this._executionPending = false;
            }
        }
    }

    /// <summary>
    /// Closes the gate. Future calls to Execute() will be deferred.
    /// </summary>
    public void CloseGate()
    {
        lock (this._lockObject)
        {
            this._gateOpen = false;
        }
    }

    /// <summary>
    /// If the gate is open, runs the action immediately.
    /// If the gate is closed, defers execution until the gate is opened.
    /// </summary>
    public void RequestExecution()
    {
        lock (this._lockObject)
        {
            if (this._gateOpen)
            {
                this._action();
            }
            else
            {
                this._executionPending = true;
            }
        }
    }
}