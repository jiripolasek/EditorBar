// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Controls;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.FocusEditorBarFileCrumbCommand)]
internal sealed class FocusEditorBarFileCrumbCommand : BaseFocusBreadcrumbCommand<FocusEditorBarFileCrumbCommand>
{
    protected override void ExecuteCore(EditorBarControl control)
    {
        control.FocusAndOpenFirstSymbolCrumb();
    }
}