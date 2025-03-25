// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents a navigation model for a location in a project.
/// Provides information about the file path, parent project, and folder structure.
/// </summary>
public sealed class LocationNavModel
{
    /// <summary>
    /// Gets the parent project information.
    /// </summary>
    public IProjectInfo Project { get; }

    /// <summary>
    /// Gets the array of parent project folder names in the hierarchy.
    /// </summary>
    public string[] ProjectFolders { get; }

    /// <summary>
    /// Gets the absolute file path of the current location.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationNavModel" /> class.
    /// </summary>
    /// <param name="project">Parent project information.</param>
    /// <param name="projectFolders">Folders inside the project.</param>
    /// <param name="filePath">Full file path.</param>
    public LocationNavModel(IProjectInfo project, string[] projectFolders, string filePath)
    {
        Requires.NotNull(project, nameof(project));
        Requires.NotNull(projectFolders, nameof(projectFolders));
        Requires.NotNull(filePath, nameof(filePath));

        this.Project = project;
        this.ProjectFolders = projectFolders;
        this.FilePath = filePath;
    }
}