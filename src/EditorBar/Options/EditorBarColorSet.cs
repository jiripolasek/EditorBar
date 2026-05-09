// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Drawing;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Represents the effective breadcrumb colors for one editor appearance mode.
/// </summary>
public sealed class EditorBarColorSet
{
    public required Color SolutionBackground { get; init; }

    public required Color SolutionForeground { get; init; }

    public required Color NonSolutionRootBackground { get; init; }

    public required Color NonSolutionRootForeground { get; init; }

    public required Color ProjectBackground { get; init; }

    public required Color ProjectForeground { get; init; }

    public required Color SolutionFolderBackground { get; init; }

    public required Color SolutionFolderForeground { get; init; }

    public required Color ParentFolderBackground { get; init; }

    public required Color ParentFolderForeground { get; init; }

    public required Color ProjectFoldersBackground { get; init; }

    public required Color ProjectFoldersForeground { get; init; }

    public required Color FileBreadcrumbBackground { get; init; }

    public required Color FileBreadcrumbForeground { get; init; }

    public required Color StructureBreadcrumbBackground { get; init; }

    public required Color StructureBreadcrumbForeground { get; init; }
}
