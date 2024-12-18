// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Threading;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Project = EnvDTE.Project;

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
    internal static async Task<Community.VisualStudio.Toolkit.Project?> ConvertToSolutionItemAsync(Project dteProject, CancellationToken cancellationToken = default)
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


    private static async Task<IEnumerable<Community.VisualStudio.Toolkit.Project>> GetAllProjectsAsync(ProjectStateFilter filter = ProjectStateFilter.Loaded)
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        IVsSolution solution = await VS.Services.GetSolutionAsync();
        IEnumerable<IVsHierarchy> hierarchies = solution.GetAllProjectHierarchies(filter);

        List<Community.VisualStudio.Toolkit.Project> list = new();

        foreach (IVsHierarchy hierarchy in hierarchies)
        {
            var si = await SolutionItem.FromHierarchyAsync(hierarchy, VSConstants.VSITEMID_ROOT);
            if (si is Community.VisualStudio.Toolkit.Project proj)
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
    internal static async Task<Project?> GetProjectFromDocumentAsync(ITextDocument document,
        CancellationToken cancellationToken = default)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        if (document.FilePath == null)
        {
            return null;
        }

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);

        var dte = (DTE?)Package.GetGlobalService(typeof(DTE));
        if (dte?.Solution?.Projects == null)
        {
            return null;
        }

        var projects = dte.Solution!.Projects.OfType<Project>().ToList();
        var projectFile = projects.FirstOrDefault(t =>
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return string.Equals(t.FullName!, document.FilePath!, StringComparison.OrdinalIgnoreCase);
        });

        if (projectFile != null)
        {
            return projectFile;
        }

        var projectItem = dte.Solution.FindProjectItem(document.FilePath);
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
    internal static async Task<List<string>> GetSolutionFolderPathAsync(SolutionItem? solutionItem, CancellationToken cancellationToken = default)
    {
        var folderPath = new List<string>();
        if (solutionItem == null)
        {
            return folderPath;
        }

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);

        var parent = solutionItem.FindParent(SolutionItemType.SolutionFolder);
        while (parent != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            folderPath.Insert(0, parent.Name);
            parent = parent.FindParent(SolutionItemType.SolutionFolder);
        }

        return folderPath;
    }
}