// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Services.StructureProviders;

namespace JPSoftworks.EditorBar.Services;

/// <summary>
/// Provides methods for navigating to specific positions within a text editor.
/// </summary>
internal interface ITextNavigationService
{
    /// <summary>
    /// Navigates to the specified position in the text.
    /// </summary>
    /// <param name="position">The position to navigate to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NavigateToPositionAsync(int position);

    /// <summary>
    /// Navigates to the specified line and column in the text.
    /// </summary>
    /// <param name="lineNumber">The line number to navigate to.</param>
    /// <param name="columnNumber">The column number to navigate to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NavigateToPositionAsync(int lineNumber, int columnNumber);

    /// <summary>
    /// Navigates to the specified anchor point.
    /// </summary>
    /// <param name="anchorPoint">The anchor point to navigate to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NavigateToAnchorAsync(AnchorPoint anchorPoint);
}