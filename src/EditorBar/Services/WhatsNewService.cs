// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Dialogs;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Services;

internal static class WhatsNewService
{
    private static bool _isShowing;

    internal static Task ShowAsync()
    {
        return ShowCoreAsync(false, isAutomatic: false);
    }

    internal static Task ShowIfPendingAsync(bool isFirstRun)
    {
        return ShowCoreAsync(isFirstRun, isAutomatic: true);
    }

    private static async Task ShowCoreAsync(bool isFirstRun, bool isAutomatic)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var options = GeneralOptionsModel.Instance;
        if (_isShowing || (isAutomatic && !options.HasUnseenWhatsNew))
        {
            return;
        }

        var openOptionsRequested = false;
        _isShowing = true;

        try
        {
            var context = new WhatsNewDisplayContext(options.VsixVersion, isFirstRun, isAutomatic);
            var dialog = new WhatsNewDialog(context);
            if (dialog.HasPages)
            {
                dialog.ShowModal();
                openOptionsRequested = dialog.OpenOptionsRequested;
            }

            await options.MarkWhatsNewAsSeenAsync();
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }
        finally
        {
            _isShowing = false;
        }

        if (openOptionsRequested)
        {
            await VS.Settings.OpenAsync<GeneralOptionPage>();
        }
    }
}
