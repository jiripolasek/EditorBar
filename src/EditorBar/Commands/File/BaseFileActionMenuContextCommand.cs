// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// Abstract base class for commands that operate on the current document within the EditorBar file action menu context.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
internal abstract class BaseFileActionMenuContextCommand<TCommand>
    : BaseMenuContextCommand<FileActionMenuContext, TCommand>
    where TCommand : class, new()
{
    /// <summary>
    /// Executes the core asynchronous operation using the provided context.
    /// </summary>
    /// <param name="context">The context containing the current document.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override Task ExecuteCoreAsync(FileActionMenuContext context)
    {
        return this.ExecuteCoreAsync(context.CurrentDocument!);
    }

    /// <summary>
    /// Executes the core asynchronous operation using the current document.
    /// </summary>
    /// <param name="currentDocument">The current document. Caller ensures that the value is not null when the method is invoked.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task ExecuteCoreAsync(ITextDocument currentDocument);
}