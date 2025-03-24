// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents a fake project that is used to display any file that are
/// </summary>
public class FileSystemProjectInfo : IProjectInfo
{
    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public string? DirectoryPath { get; }

    /// <inheritdoc />
    public bool ImplicitProject => true;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemProjectInfo" /> class.
    /// </summary>
    /// <param name="displayName">The display name of the project.</param>
    /// <param name="path">The directory path of the project, if available.</param>
    public FileSystemProjectInfo(string displayName, string? path)
    {
        Requires.NotNullOrWhiteSpace(displayName, nameof(displayName));

        this.DisplayName = displayName;
        this.DirectoryPath = path;
    }
}