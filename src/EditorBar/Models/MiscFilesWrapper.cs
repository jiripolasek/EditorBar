// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.IO;

namespace JPSoftworks.EditorBar.Models;

public class MiscFilesWrapper : SolutionProject
{
    public MiscFilesWrapper(string? solutionPath)
    {
        this.DisplayName = "Miscellaneous Files";
        this.DirectoryPath = Path.GetDirectoryName(solutionPath ?? "");
    }
}