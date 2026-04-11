// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.IO;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using Microsoft;
using Microsoft.CodeAnalysis;
using CodeAnalysisProject = Microsoft.CodeAnalysis.Project;
using TookitSolution = Community.VisualStudio.Toolkit.Solution;
using ToolkitProject = Community.VisualStudio.Toolkit.Project;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents a project wrapper that contains project and solution information, including IntelliSense contexts and
/// solution folders.
/// It provides methods to create project info asynchronously and update IntelliSense context.
/// </summary>
public class GenericProjectInfo : BaseSolutionProjectInfo, IHasSolutionFolders
{
    /// <summary>
    /// Gets the project in the solution.
    /// </summary>
    public ToolkitProject Project { get; }

    /// <summary>
    /// Gets the VS solution.
    /// </summary>
    public TookitSolution? Solution { get; }

    /// <summary>
    /// Gets the IntelliSense contexts for a Code Analysis project.
    /// </summary>
    public CodeAnalysisProject? IntelliSenseContexts { get; private set; }

    /// <summary>
    /// Gets the IntelliSense alternative context documents for a Code Analysis project.
    /// </summary>
    public IReadOnlyList<Document> IntelliSenseAlternativeContextsDocuments { get; private set; } = [];

    /// <summary>
    /// Gets the solution folder names.
    /// </summary>
    public IReadOnlyList<string> SolutionFolders { get; }

    private GenericProjectInfo(ToolkitProject project, TookitSolution? solution, IReadOnlyList<string> solutionFolders)
        : base(project.Name, Path.GetDirectoryName(project.FullPath!))
    {
        Requires.NotNull(project);
        Requires.NotNull(solutionFolders);

        this.Project = project;
        this.Solution = solution!;
        this.SolutionFolders = solutionFolders;
    }

    /// <summary>
    /// Creates a project information object asynchronously from a given project.
    /// </summary>
    /// <param name="project">The project to create information from.</param>
    /// <returns>An object containing information about the specified project.</returns>
    public static async Task<IProjectInfo> CreateFromProjectAsync(ToolkitProject project)
    {
        Requires.NotNull(project);

        var solution = await VS.Solutions.GetCurrentSolutionAsync();
        var solutionFolders = await GetSolutionFoldersAsync(project);
        return new GenericProjectInfo(project, solution, solutionFolders);
    }

    /// <summary>
    /// Updates the IntelliSense context based on the provided project information.
    /// </summary>
    /// <param name="projects">Information about the active project and alternative context documents for IntelliSense.</param>
    public void UpdateIntelliSenseContext(IntelliSenseProjectContextContainer? projects)
    {
        if (projects == null)
        {
            this.IntelliSenseContexts = null;
            this.IntelliSenseAlternativeContextsDocuments = [];
            this.DisplayName = this.Project.Name;
        }
        else
        {
            this.IntelliSenseContexts = projects.Value.ActiveProject;
            this.IntelliSenseAlternativeContextsDocuments = projects.Value.AlternativeContextDocuments;
            this.DisplayName = projects.Value.ActiveProject != null
                ? projects.Value.ActiveProject.Name
                : this.Project.Name;
        }
    }

    private static async Task<List<string>> GetSolutionFoldersAsync(SolutionItem? solutionItem)
    {
        return solutionItem == null ? [] : await VisualStudioHelper.GetSolutionFolderPathAsync(solutionItem);
    }
}
