// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Defines file-related actions that can be executed on text or code documents.
/// </summary>
public enum FileAction
{
    /// <summary>
    /// No action.
    /// </summary>
    None,

    /// <summary>
    /// Opens the folder containing the document.
    /// </summary>
    [Description("Open containing folder")]
    OpenContainingFolder,

    /// <summary>
    /// Opens the document in an external editor.
    /// </summary>
    [Description("Open in external editor")]
    OpenInExternalEditor,

    /// <summary>
    /// Copies the relative path of the document to the clipboard.
    /// </summary>
    [Description("Copy relative path to Clipboard")]
    CopyRelativePath,

    /// <summary>
    /// Copies the absolute path of the document to the clipboard.
    /// </summary>
    [Description("Copy full path to Clipboard")]
    CopyAbsolutePath,

    /// <summary>
    /// Opens the document in its default associated editor.
    /// </summary>
    [Description("Open default editor")]
    OpenInDefaultEditor
}