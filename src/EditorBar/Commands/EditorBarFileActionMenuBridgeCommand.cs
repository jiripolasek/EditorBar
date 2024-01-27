// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// A base class that makes it easier to handle commands that requires a current document. Uses <see cref="EditorBarFileActionMenuBridge"/> to get the current document.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="BaseCommand{T}" />
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
internal abstract class EditorBarFileActionMenuBridgeCommand<T> : BaseCommand<T> where T : class, new()
{
    private EditorBarFileActionMenuBridge? _bridge;

    /// <summary>Allows the implementor to manipulate the command before its execution.</summary>
    /// <remarks>
    /// This method is invoked right after the <see cref="M:Community.VisualStudio.Toolkit.BaseCommand`1.InitializeAsync(Microsoft.VisualStudio.Shell.AsyncPackage)" /> method is executed and allows you to
    /// manipulate the <see cref="P:Community.VisualStudio.Toolkit.BaseCommand.Command" /> property etc. as part of the initialization phase.
    /// </remarks>
    /// <exception cref="ServiceUnavailableException">Either the service could not be acquired, or the service does not support the requested interface.</exception>
    protected override async Task InitializeCompletedAsync()
    {
        this._bridge = await this.Package.GetServiceAsync<EditorBarFileActionMenuBridge, EditorBarFileActionMenuBridge>();
    }

    /// <summary>Executes asynchronously when the command is invoked and <see cref="M:Community.VisualStudio.Toolkit.BaseCommand.Execute(System.Object,System.EventArgs)" /> isn't overridden.</summary>
    /// <remarks>Use this method instead of <see cref="M:Community.VisualStudio.Toolkit.BaseCommand.Execute(System.Object,System.EventArgs)" /> if you're invoking any async tasks by using async/await patterns.</remarks>
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        var currentDocument = this._bridge?.CurrentDocument;
        return currentDocument == null ? Task.CompletedTask : this.ExecuteCoreAsync(currentDocument);
    }

    /// <summary>
    /// Executes the core asynchronous.
    /// </summary>
    /// <param name="currentDocument">The current document. Caller ensures that the value is not null when the method is invoked.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task ExecuteCoreAsync(ITextDocument currentDocument);
}