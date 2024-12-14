// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Threading;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Project = EnvDTE.Project;

namespace JPSoftworks.EditorBar;

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

        var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All).ConfigureAwait(false);
        return allProjects.FirstOrDefault(t =>
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return t.FullPath == dteProject.FullName;
        });
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
    /// <param name="project">The project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown back at the awaiting caller if <paramref name="cancellationToken" /> is canceled,
    /// even if the caller is already on the main thread.
    /// </exception>
    internal static async Task<List<string>> GetSolutionFolderPathAsync(SolutionItem? project,
        CancellationToken cancellationToken = default)
    {
        var folderPath = new List<string>();
        if (project == null)
        {
            return folderPath;
        }

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);

        var parent = project.FindParent(SolutionItemType.SolutionFolder);
        while (parent != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            folderPath.Insert(0, parent.Name);
            parent = parent.FindParent(SolutionItemType.SolutionFolder);
        }

        return folderPath;
    }
}