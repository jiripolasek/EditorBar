// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents information about a project.
/// </summary>
public interface IProjectInfo
{
    /// <summary>
    /// Gets the display name of the project.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the directory path of the project, if available.
    /// </summary>
    public string? DirectoryPath { get; }

    /// <summary>
    /// Gets a value indicating whether the project is implicit.
    /// </summary>
    public bool ImplicitProject { get; }
}