// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Project = EnvDTE.Project;

namespace JPSoftworks.EditorBar;

internal static class VisualStudioHelper
{
    internal static async Task<SolutionItem?> ConvertToSolutionItemAsync(Project dteProject)
    {
        if (dteProject == null)
        {
            throw new ArgumentNullException(nameof(dteProject));
        }

        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All).ConfigureAwait(false);
        return allProjects.FirstOrDefault(t => t.FullPath == dteProject.FullName);
    }

    internal static async Task<Project?> GetProjectFromDocumentAsync(ITextDocument document)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var dte = (DTE)Package.GetGlobalService(typeof(DTE));
        var projects = dte.Solution.Projects.OfType<Project>().ToList();
        var projectFile = projects.FirstOrDefault(t => string.Equals(t.FullName, document.FilePath, StringComparison.OrdinalIgnoreCase));
        if (projectFile != null)
        {
            return projectFile;
        }

        var projectItem = dte.Solution.FindProjectItem(document.FilePath);
        return projectItem?.ContainingProject;
    }

    internal static async Task<List<string>> GetSolutionFolderPathAsync(SolutionItem? project)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var folderPath = new List<string>();
        if (project == null)
        {
            return folderPath;
        }

        var parent = project.FindParent(SolutionItemType.SolutionFolder);
        while (parent != null)
        {
            folderPath.Add(parent.Name);
            parent = parent.FindParent(SolutionItemType.SolutionFolder);
        }

        // Reverse to get the order from solution to project
        folderPath.Reverse();
        return folderPath;
    }
}