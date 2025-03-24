// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Represents how the UI displays the name of the current document.
/// </summary>
public enum FileLabel
{
    /// <summary>
    /// Displays the absolute path of the file.
    /// </summary>
    [Description("Absolute Path")]
    AbsolutePath,

    /// <summary>
    /// Displays the relative path of the file within the project.
    /// </summary>
    [Description("Relative Path (in project)")]
    RelativePathInProject,

    /// <summary>
    /// Displays the relative path of the file within the solution.
    /// </summary>
    [Description("Relative Path (in solution)")]
    RelativePathInSolution,

    /// <summary>
    /// Displays only the file name.
    /// </summary>
    [Description("File Name")]
    FileName,

    /// <summary>
    /// Hides the file label.
    /// </summary>
    [Description("Hidden")]
    Hidden
}