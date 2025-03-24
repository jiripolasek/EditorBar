// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Threading;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Resources;
using Microsoft;
using Microsoft.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Provides a way to create a <see cref="LocationNavModel" /> for the current location in the editor using Visual Studio Community Tookit APIs.
/// </summary>
/// <seealso cref="ILocationProvider" />
public class ToolkitLocationProvider : ILocationProvider
{
    private const string MiscProjectKindGuid = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
    private static readonly char[] DirectorySeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

    /// <inheritdoc cref="ILocationProvider.CreateAsync" />
    public async Task<LocationNavModel?> CreateAsync(
        IWpfTextView wpfTextView,
        CancellationToken cancellationToken = default)
    {
        Requires.NotNull(wpfTextView, nameof(wpfTextView));

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);

        var document = wpfTextView.TextBuffer!.GetTextDocument() ?? wpfTextView.GetTextDocumentFromDocumentBuffer();
        if (document == null)
        {
            return null;
        }

        // get project wrapper
        var projectWrapper = await GetProjectWrapperAsync(document);

        // get project folder paths
        var inProjectPathElements = GetInProjectPathParts(projectWrapper.DirectoryPath, document);

        return new LocationNavModel(projectWrapper, inProjectPathElements, document.FilePath!);
    }

    private static async Task<IProjectInfo> GetProjectWrapperAsync(ITextDocument document)
    {
        Requires.NotNull(document, nameof(document));

        // find in other projects (e.g. misc files) that are not covered by Toolkit Project and thus not by PhysicalDocument.ContainingProject
        // handle solution root folder (for the files that are in solution folder, but not part of the solution or project itself)
        // - add as a file system root instead of solution root to mark that the file is not directly part of solution
        return await CreateFromPhysicalDocumentOrDefaultAsync(document)
               ?? await FindParentProjectAsync(document)
               ?? await FindFileInSolutionAsync(document) // fallback for all other external paths
               ?? FindInKnownFakeRoots(document) // handle known fake roots (like temp, app data, etc.)
               ?? CreateFallbackFileSystemProjectWrapper(document);
    }

    private static IProjectInfo CreateFallbackFileSystemProjectWrapper(ITextDocument document)
    {
        var projectPath = Path.GetDirectoryName(document.FilePath);

        if (projectPath == null)
        {
            return new NullProjectInfo();
        }

        var displayName = PathUtils.IsNetworkPath(document.FilePath!)
            ? Strings.NetworkRootName
            : Strings.LocalDriveRootName; // network path
        return new FileSystemProjectInfo(displayName ?? "", null);
    }

    private static IProjectInfo? FindInKnownFakeRoots(ITextDocument document)
    {
        return KnownFakeRoots
                .FakeRoots
                .FirstOrDefault(fr => document.FilePath!.StartsWith(fr.Path, StringComparison.OrdinalIgnoreCase))
            is { } matched
            ? new FileSystemProjectInfo(matched.DisplayName, matched.Path)
            : null;
    }

    private static async Task<IProjectInfo?> FindFileInSolutionAsync(ITextDocument document)
    {
        // if the file is in the solution folder, but not part of the solution or project itself
        var solution = await VS.Solutions.GetCurrentSolutionAsync();
        if (solution == null)
        {
            return null;
        }

        var solutionPath = Path.GetDirectoryName(solution.FullPath!);
        if (solutionPath == null || !document.FilePath!.StartsWith(solutionPath))
        {
            return null;
        }

        return new FileSystemProjectInfo(Strings.SolutionRootName ?? "", solutionPath);
    }

    private static async Task<IProjectInfo?> CreateFromPhysicalDocumentOrDefaultAsync(ITextDocument document)
    {
        var physicalDocument = await PhysicalFile.FromFileAsync(document.FilePath!);
        var project = physicalDocument?.ContainingProject;
        return project != null ? await GenericProjectInfo.CreateFromProjectAsync(project) : null;
    }

    private static string[] GetInProjectPathParts(string? projectDirectoryPath, ITextDocument document)
    {
        string[] inProjectPathElements;
        if (projectDirectoryPath != null)
        {
            var relativePath =
                PathUtils.GetRelativePath(projectDirectoryPath, Path.GetDirectoryName(document.FilePath!)!);
            inProjectPathElements = relativePath.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            var absolutePath = Path.GetDirectoryName(document.FilePath!) ?? "";
            inProjectPathElements = absolutePath.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        return inProjectPathElements;
    }

    private static async Task<IProjectInfo?> FindParentProjectAsync(ITextDocument document)
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        var dteProject = await VisualStudioHelper.GetProjectFromDocumentAsync(document);

        if (dteProject != null)
        {
            // is misc:
            if (dteProject.Kind == MiscProjectKindGuid)
            {
                var currentSolution = await VS.Solutions.GetCurrentSolutionAsync();
                return new MiscFilesProjectInfo(currentSolution?.FullPath);
            }

            var project2 = await VisualStudioHelper.ConvertToSolutionItemAsync(dteProject);
            if (project2 != null)
            {
                return await GenericProjectInfo.CreateFromProjectAsync(project2);
            }
        }

        // handle case when the document is a project file itself
        var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
        var project = allProjects.FirstOrDefault(t => t.FullPath == document.FilePath);
        return project == null ? null : await GenericProjectInfo.CreateFromProjectAsync(project);
    }
}