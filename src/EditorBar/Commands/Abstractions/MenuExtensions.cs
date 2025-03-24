// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands.Abstractions;

/// <summary>
/// Extension methods for showing context menus
/// </summary>
internal static class MenuExtensions
{
    /// <summary>
    /// Shows a menu for the given context with error handling
    /// </summary>
    public static void ShowMenu(this MenuContext context, Action<Exception>? errorHandler = null)
    {
        ThreadHelper.JoinableTaskFactory!.RunAsync(async () =>
        {
            try
            {
                var contextService = await VS.GetRequiredServiceAsync<IMenuContextService, IMenuContextService>();
                await contextService.ShowMenuAsync(context);
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    errorHandler(ex);
                }
                else
                {
                    await ex.LogAsync();
                }
            }
        }).FireAndForget();
    }
}