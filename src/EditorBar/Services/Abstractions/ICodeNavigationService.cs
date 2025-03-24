// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Services;

/// <summary>
/// Provides navigation services for code elements.
/// </summary>
internal interface ICodeNavigationService
{
    /// <summary>
    /// Navigates to the declaration of the specified symbol in the current view.
    /// </summary>
    /// <param name="symbol">The symbol to navigate to.</param>
    /// <param name="location">An optinal location of the symbol, used in case there are multiple definitions of the symbol.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NavigateToSymbolDeclarationInCurrentViewAsync(ISymbol symbol, Location? location = null);

    /// <summary>
    /// Navigates to the specified syntax node in the current view.
    /// </summary>
    /// <param name="declarationSyntax">The syntax node to navigate to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NavigateToSyntaxNodeInCurrentViewAsync(SyntaxNode declarationSyntax);
}