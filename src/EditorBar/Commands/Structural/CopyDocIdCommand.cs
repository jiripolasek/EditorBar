// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarStructureBreadcrumbMenuCopyDocIdCommand)]
internal sealed class CopyDocIdCommand : BaseStructureMenuContextCommand<CopyDocIdCommand>
{
    protected override async Task ExecuteCoreAsync(BaseStructureModel baseStructureModel, IWpfTextView wpfTextView)
    {
        if (baseStructureModel.AnchorPoint is not SymbolAnchorPoint symbolAnchorPoint)
        {
            return;
        }

        var docId = symbolAnchorPoint.Symbol.GetDocumentationCommentId();
        if (docId != null)
        {
            await ClipboardHelper.SetTextAsync(docId, "Documentation Comment ID copied to clipboard");
        }
    }
}