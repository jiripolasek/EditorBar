// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Services.LocationProviders;

namespace JPSoftworks.EditorBar.Commands;

internal static class ProjectLocationHelper
{
    internal static string? GetProjectLaunchPath(IProjectInfo project)
    {
        return project is GenericProjectInfo projectWrapper &&
               !string.IsNullOrWhiteSpace(projectWrapper.Project.FullPath)
            ? projectWrapper.Project.FullPath
            : project.DirectoryPath;
    }
}
