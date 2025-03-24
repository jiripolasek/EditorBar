// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarStructureBreadcrumbMenuCopyFullName)]
internal sealed class CopyFullNameCommand : BaseStructureMenuContextCommand<CopyFullNameCommand>
{
    protected override async Task ExecuteCoreAsync(BaseStructureModel baseStructureModel, IWpfTextView wpfTextView)
    {
        if (baseStructureModel.AnchorPoint is not SymbolAnchorPoint anchorPoint)
        {
            return;
        }

        var fullyQualifiedName = anchorPoint.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        Clipboard.SetText(fullyQualifiedName);
        await VS.StatusBar.ShowMessageAsync("Full Name copied to clipboard");
    }
}