// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.ComponentModel;

namespace JPSoftworks.EditorBar.Options;

public enum FileLabel
{
    [Description("Absolute Path")]
    AbsolutePath,

    [Description("Relative Path (in project)")]
    RelativePathInProject,

    [Description("Relative Path (in solution)")]
    RelativePathInSolution,

    [Description("File Name")]
    FileName
}