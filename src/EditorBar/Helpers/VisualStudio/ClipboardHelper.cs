// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides helper methods for interacting with the clipboard in Visual Studio.
/// </summary>
internal static class ClipboardHelper
{
    /// <summary>
    /// Sets the specified text to the clipboard and shows a message in the Visual Studio status bar.
    /// </summary>
    /// <param name="text">
    /// The text to set to the clipboard. If null or whitespace, a message will be shown indicating nothing
    /// to copy.
    /// </param>
    /// <param name="message">The message to show in the Visual Studio status bar after setting the text to the clipboard.</param>
    public static async Task SetTextAsync(string? text, string message)
    {
        if (string.IsNullOrWhiteSpace(text!))
        {
            await VS.StatusBar.ShowMessageAsync("Nothing to copy to Clipboard");
            return;
            // return Task.CompletedTask;
        }

        try
        {
            await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();
            Clipboard.SetText(text!);
            await VS.StatusBar.ShowMessageAsync(message);
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }
    }
}