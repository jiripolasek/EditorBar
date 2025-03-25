// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Threading;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using DteProject = EnvDTE.Project;
using Project = Community.VisualStudio.Toolkit.Project;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Helper methods for operations on Visual Studio objects.
/// </summary>
internal static class VisualStudioHelper
{
    /// <summary>
    /// Converts to solution item asynchronous.
    /// </summary>
    /// <param name="dteProject">The DTE project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">dteProject</exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown back at the awaiting caller if <paramref name="cancellationToken" /> is canceled,
    /// even if the caller is already on the main thread.
    /// </exception>
    internal static async Task<Project?> ConvertToSolutionItemAsync(
        DteProject dteProject,
        CancellationToken cancellationToken = default)
    {
        if (dteProject == null)
        {
            throw new ArgumentNullException(nameof(dteProject));
        }

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);
        var allProjects = await GetAllProjectsAsync(ProjectStateFilter.All).ConfigureAwait(false);
        return allProjects.FirstOrDefault(project =>
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return project.FullPath == dteProject.FullName;
        });
    }


    private static async Task<IEnumerable<Project>> GetAllProjectsAsync(
        ProjectStateFilter filter = ProjectStateFilter.Loaded)
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        var solution = await VS.Services.GetSolutionAsync();
        var hierarchies = solution.GetAllProjectHierarchies(filter);

        List<Project> list = [];

        foreach (var hierarchy in hierarchies)
        {
            var si = await SolutionItem.FromHierarchyAsync(hierarchy, VSConstants.VSITEMID_ROOT);
            if (si is Project proj)
            {
                list.Add(proj);
            }
        }

        return list;
    }

    /// <summary>
    /// Gets the project the <paramref name="document" /> belongs to.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The instance of project the document belongs to or <c>null</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">document</exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown back at the awaiting caller if <paramref name="cancellationToken" /> is canceled, even if the caller is
    /// already on the main thread.
    /// </exception>
    internal static async Task<DteProject?> GetProjectFromDocumentAsync(
        ITextDocument document,
        CancellationToken cancellationToken = default)
    {
        Requires.NotNull(document, nameof(document));
        return await GetProjectFromPathAsync(cancellationToken, document.FilePath);
    }

    private static async Task<DteProject?> GetProjectFromPathAsync(
        CancellationToken cancellationToken,
        string? filePath)
    {
        if (filePath == null)
        {
            return null;
        }

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);

        var dte = (DTE?)Package.GetGlobalService(typeof(DTE));
        if (dte?.Solution?.Projects == null)
        {
            return null;
        }

        var projects = dte.Solution!.Projects.OfType<DteProject>().ToList();
        var projectFile = projects.FirstOrDefault(t =>
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return string.Equals(t.FullName!, filePath, StringComparison.OrdinalIgnoreCase);
        });

        if (projectFile != null)
        {
            return projectFile;
        }

        var projectItem = dte.Solution.FindProjectItem(filePath);
        return projectItem?.ContainingProject;
    }

    /// <summary>
    /// Gets the solution folder path asynchronous.
    /// </summary>
    /// <param name="solutionItem">The project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown back at the awaiting caller if <paramref name="cancellationToken" /> is canceled,
    /// even if the caller is already on the main thread.
    /// </exception>
    internal static async Task<List<string>> GetSolutionFolderPathAsync(
        SolutionItem solutionItem,
        CancellationToken cancellationToken = default)
    {
        Requires.NotNull(solutionItem, nameof(solutionItem));

        var folderPath = new List<string>();

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);

        var parent = solutionItem.FindParent(SolutionItemType.SolutionFolder);
        while (parent != null)
        {
            folderPath.Insert(0, parent.Name);
            parent = parent.FindParent(SolutionItemType.SolutionFolder);
        }

        return folderPath;
    }
}