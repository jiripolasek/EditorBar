// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Controls;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.FocusEditorBarInnermostTypeCrumbCommand)]
internal sealed class FocusEditorBarInnermostTypeCrumbCommand
    : BaseFocusBreadcrumbCommand<FocusEditorBarInnermostTypeCrumbCommand>
{
    protected override void ExecuteCore(EditorBarControl control)
    {
        control.FocusAndOpenLastSymbolCrumb();
    }
}