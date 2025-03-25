// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents an abstract base class for project that is
/// part of a Visual Studio solution.
/// </summary>
public abstract class BaseSolutionProjectInfo : IProjectInfo
{
    /// <summary>
    /// Gets the display name of the project.
    /// </summary>
    public string DisplayName { get; protected set; }

    /// <summary>
    /// Gets the directory path of the project.
    /// </summary>
    public string? DirectoryPath { get; protected init; }

    /// <summary>
    /// Gets a value indicating whether the project is implicit.
    /// </summary>
    public virtual bool ImplicitProject { get; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSolutionProjectInfo" /> class.
    /// </summary>
    /// <param name="displayName">The display name of the project.</param>
    /// <param name="directoryPath">The directory path of the project.</param>
    protected BaseSolutionProjectInfo(string displayName, string? directoryPath)
    {
        Requires.NotNullOrEmpty(displayName, nameof(displayName));

        this.DisplayName = displayName;
        this.DirectoryPath = directoryPath;
    }
}