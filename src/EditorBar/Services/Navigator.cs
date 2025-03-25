// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services.StructureProviders;

namespace JPSoftworks.EditorBar.Services;

internal static class Navigator
{
    public static async Task NavigateToAnchorAsync(AnchorPoint anchorPoint)
    {
        var documentView = await TryOpenDocumentViewAsync(anchorPoint.FilePath);
        if (documentView is { TextView: not null })
        {
            await documentView.TextView.GetTextNavigationService().NavigateToAnchorAsync(anchorPoint);
            if (documentView.WindowFrame != null)
            {
                await documentView.WindowFrame.ShowAsync();
            }
        }
    }

    private static async Task<DocumentView?> TryOpenDocumentViewAsync(string file)
    {
        var isOpen = await VS.Documents.IsOpenAsync(file);
        if (!isOpen)
        {
            await VS.Documents.OpenAsync(file);
        }

        return await VS.Documents.GetDocumentViewAsync(file);
    }
}