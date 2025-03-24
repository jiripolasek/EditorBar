// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.IO;
using JPSoftworks.EditorBar.Resources;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents a wrapper for miscellaneous files in a VS solution.
/// </summary>
public class MiscFilesProjectInfo : BaseSolutionProjectInfo
{
    /// <inheritdoc />
    public override bool ImplicitProject => false;

    /// <summary>
    /// Initializes a new instance of the MiscFilesWrapper class, setting the name and directory based on the provided
    /// path.
    /// </summary>
    /// <param name="solutionPath">Specifies the path to the solution, which is used to determine the directory for miscellaneous files.</param>
    public MiscFilesProjectInfo(string? solutionPath)
        : base(Strings.MiscellaneousFilesName!, Path.GetDirectoryName(solutionPath ?? ""))
    {
    }
}