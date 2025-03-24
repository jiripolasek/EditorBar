// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Controls;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.FocusEditorBarProjectsCrumbCommand)]
internal sealed class
    FocusEditorBarProjectBreadcrumbCommand : BaseFocusBreadcrumbCommand<FocusEditorBarProjectBreadcrumbCommand>
{
    protected override void ExecuteCore(EditorBarControl control)
    {
        control.FocusAndOpenProjectCrumb();
    }
}