// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.ComponentModel;

namespace JPSoftworks.EditorBar.Options;

public enum FileAction
{
    None,

    [Description("Open containing folder")]
    OpenContainingFolder,

    [Description("Open in external editor")]
    OpenInExternalEditor,

    [Description("Copy relative path to ClipboardX")]
    CopyRelativePath,

    [Description("Copy full path to Clipboard")]
    CopyAbsolutePath,

    [Description("Open default editor")]
    OpenInDefaultEditor
}