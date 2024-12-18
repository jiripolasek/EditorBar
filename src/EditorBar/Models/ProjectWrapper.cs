// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.IO;
using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Models;

public class ProjectWrapper : SolutionProject
{
    public Project Project { get; }

    public ProjectWrapper(Project project)
    {
        this.Project = project;
        this.DisplayName = project.Name;
        this.DirectoryPath = Path.GetDirectoryName(project.FullPath!);
    }
}