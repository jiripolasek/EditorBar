// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.IO;
using System.Windows.Input;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Services.LocationProviders;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Project = Community.VisualStudio.Toolkit.Project;

namespace JPSoftworks.EditorBar.Helpers;

internal static class LocationBreadcrumbTreeBuilder
{
    private static readonly FileNameToImageMonikerConverter FileNameToImageMonikerConverter = new();

    public static async Task<IList<MemberTreeItemViewModel>> CreateSolutionRootItemsAsync(IWpfTextView currentTextView)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var solution = await VS.Solutions.GetCurrentSolutionAsync();
        if (solution != null)
        {
            return CreateSolutionItems(solution.Children.OfType<SolutionItem>(), currentTextView);
        }

        var rootItems = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
        return CreateSolutionItems(rootItems.OfType<SolutionItem>(), currentTextView);
    }

    public static async Task<IList<MemberTreeItemViewModel>> CreateProjectItemsAsync(
        GenericProjectInfo projectInfo,
        ICommand? switchProjectCommand,
        IWpfTextView currentTextView)
    {
        Requires.NotNull(projectInfo);
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var items = new List<MemberTreeItemViewModel>();

        if (projectInfo.IntelliSenseAlternativeContextsDocuments.Count > 1 && switchProjectCommand != null)
        {
            items.AddRange(CreateAlternativeProjectNodes(projectInfo, switchProjectCommand));
        }

        items.AddRange(CreateSolutionItems(projectInfo.Project.Children.OfType<SolutionItem>(), currentTextView));
        return items;
    }

    public static async Task<IList<MemberTreeItemViewModel>> CreateSolutionItemChildrenAsync(
        SolutionItem solutionItem,
        IWpfTextView currentTextView)
    {
        Requires.NotNull(solutionItem);
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        return CreateSolutionItems(solutionItem.Children.OfType<SolutionItem>(), currentTextView);
    }

    public static Task<IList<MemberTreeItemViewModel>> CreateDirectoryItemsAsync(
        string directoryPath,
        IWpfTextView currentTextView)
    {
        Requires.NotNullOrWhiteSpace(directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            return Task.FromResult<IList<MemberTreeItemViewModel>>([]);
        }

        var directories = Directory.EnumerateDirectories(directoryPath)
            .OrderBy(static path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
            .Select(path => CreateDirectoryNode(path, currentTextView));

        var files = Directory.EnumerateFiles(directoryPath)
            .OrderBy(static path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
            .Select(path => CreateFileNode(path, currentTextView));

        return Task.FromResult<IList<MemberTreeItemViewModel>>([.. directories, .. files]);
    }

    private static List<MemberTreeItemViewModel> CreateSolutionItems(
        IEnumerable<SolutionItem> solutionItems,
        IWpfTextView currentTextView)
    {
        return solutionItems
            .Where(ShouldIncludeSolutionItem)
            .Select(item => CreateSolutionItemNode(item, currentTextView))
            .ToList();
    }

    private static IEnumerable<MemberTreeItemViewModel> CreateAlternativeProjectNodes(
        GenericProjectInfo projectInfo,
        ICommand switchProjectCommand)
    {
        return projectInfo.IntelliSenseAlternativeContextsDocuments
            .OrderBy(static document => document.Project.Name, StringComparer.OrdinalIgnoreCase)
            .Select(document => new MemberTreeItemViewModel
            {
                PrimaryName = document.Project.Name,
                SearchText = document.Project.Name,
                ImageMoniker = VsImageHelper.GetImageMonikers(IconIds.PartialDocumentCount),
                Command = switchProjectCommand,
                CommandParameter = document
            });
    }

    private static MemberTreeItemViewModel CreateSolutionItemNode(SolutionItem item, IWpfTextView currentTextView)
    {
        Requires.NotNull(item);

        var canHaveChildren = CanHaveChildren(item);
        var primaryName = string.IsNullOrWhiteSpace(item.Text) ? item.Name : item.Text;
        var node = new MemberTreeItemViewModel
        {
            PrimaryName = primaryName,
            SearchText = string.Join(" ", new[] { primaryName, item.Name, item.FullPath }.Where(static value => !string.IsNullOrWhiteSpace(value))),
            ImageMoniker = GetSolutionItemMoniker(item),
            ChildrenProvider = canHaveChildren ? () => CreateSolutionItemChildrenAsync(item, currentTextView) : null,
            ExpandOnActivate = ShouldExpandOnActivate(item, canHaveChildren),
            Command = canHaveChildren && !IsActivatableFileItem(item) ? null : CreateLeafCommand(item),
            ContextCommand = CreateContextCommand(item, primaryName, currentTextView),
            InvokeOnActivate = IsActivatableFileItem(item)
        };

        node.PrepareForDisplay();
        return node;
    }

    private static MemberTreeItemViewModel CreateDirectoryNode(string directoryPath, IWpfTextView currentTextView)
    {
        var hasEntries = Directory.EnumerateFileSystemEntries(directoryPath).Any();
        var directoryName = Path.GetFileName(directoryPath);
        var model = new PhysicalDirectoryModel(string.IsNullOrWhiteSpace(directoryName) ? directoryPath : directoryName, directoryPath);
        var node = new MemberTreeItemViewModel
        {
            PrimaryName = model.Name,
            SearchText = directoryPath,
            ImageMoniker = KnownMonikers.FolderOpened,
            ExpandOnActivate = true,
            ChildrenProvider = hasEntries ? () => CreateDirectoryItemsAsync(directoryPath, currentTextView) : null,
            ContextCommand = new DispatchedDelegateCommand(_ => new LocationBreadcrumbMenuContext(model, currentTextView).ShowMenu())
        };

        node.PrepareForDisplay();
        return node;
    }

    private static MemberTreeItemViewModel CreateFileNode(string filePath, IWpfTextView currentTextView)
    {
        return new MemberTreeItemViewModel
        {
            PrimaryName = Path.GetFileName(filePath),
            SearchText = filePath,
            ImageMoniker = GetFileMoniker(filePath),
            Command = new DispatchedDelegateCommand(_ => OpenDocumentAsync(filePath).FireAndForget()),
            ContextCommand = new DispatchedDelegateCommand(_ => new FileActionMenuContext(filePath).ShowMenu())
        };
    }

    private static ICommand? CreateContextCommand(SolutionItem item, string primaryName, IWpfTextView currentTextView)
    {
        if (item is Project project)
        {
            return new DispatchedDelegateCommand(_ => ShowProjectMenuAsync(project, currentTextView).FireAndForget());
        }

        if (item.Type == SolutionItemType.SolutionFolder)
        {
            return new DispatchedDelegateCommand(_ => new SolutionFolderBreadcrumbMenuContext(item, currentTextView).ShowMenu());
        }

        if (IsActivatableFileItem(item))
        {
            return new DispatchedDelegateCommand(_ => new FileActionMenuContext(item.FullPath).ShowMenu());
        }

        if (!string.IsNullOrWhiteSpace(item.FullPath) && Directory.Exists(item.FullPath))
        {
            var model = new PhysicalDirectoryModel(primaryName, item.FullPath);
            return new DispatchedDelegateCommand(_ => new LocationBreadcrumbMenuContext(model, currentTextView).ShowMenu());
        }

        return null;
    }

    private static ICommand CreateLeafCommand(SolutionItem item)
    {
        return new DispatchedDelegateCommand(_ =>
        {
            if (item is not Project &&
                item.FullPath is { Length: > 0 } fullPath &&
                File.Exists(fullPath))
            {
                OpenDocumentAsync(fullPath).FireAndForget();
                return;
            }

            item.SelectInSolutionExplorerAsync().FireAndForget();
        });
    }

    private static bool CanHaveChildren(SolutionItem item)
    {
        Requires.NotNull(item);
        return item is Project || item.Children.OfType<SolutionItem>().Any(ShouldIncludeSolutionItem);
    }

    private static bool ShouldIncludeSolutionItem(SolutionItem item)
    {
        Requires.NotNull(item);

        if (item.IsNonVisibleItem)
        {
            return false;
        }

        return item.Type switch
        {
            SolutionItemType.PhysicalFile => HasExistingFile(item),
            SolutionItemType.PhysicalFolder => true,
            SolutionItemType.Project => true,
            SolutionItemType.SolutionFolder => true,
            SolutionItemType.VirtualFolder => ContainsBrowsableDescendants(item),
            SolutionItemType.VirtualProject => ContainsBrowsableDescendants(item),
            SolutionItemType.MiscProject => ContainsBrowsableDescendants(item),
            SolutionItemType.Unknown => ContainsBrowsableDescendants(item),
            _ => ContainsBrowsableDescendants(item)
        };
    }

    private static bool ContainsBrowsableDescendants(SolutionItem item)
    {
        return item.Children
            .OfType<SolutionItem>()
            .Any(child => ShouldIncludeSolutionItem(child));
    }

    private static bool HasExistingFile(SolutionItem item)
    {
        var fullPath = item.FullPath;
        return !string.IsNullOrWhiteSpace(fullPath) && File.Exists(fullPath);
    }

    private static bool IsActivatableFileItem(SolutionItem item)
    {
        return item.Type == SolutionItemType.PhysicalFile && HasExistingFile(item);
    }

    private static bool ShouldExpandOnActivate(SolutionItem item, bool canHaveChildren)
    {
        return canHaveChildren && !IsActivatableFileItem(item);
    }

    private static StackedImageMoniker GetSolutionItemMoniker(SolutionItem item)
    {
        if (item.FullPath is { Length: > 0 } fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                return KnownMonikers.FolderOpened;
            }

            if (File.Exists(fullPath))
            {
                return GetFileMoniker(fullPath);
            }
        }

        return item.Type switch
        {
            SolutionItemType.Project => KnownMonikers.CSProjectNode,
            SolutionItemType.VirtualProject => KnownMonikers.CSProjectNode,
            SolutionItemType.SolutionFolder => KnownMonikers.FolderOpened,
            SolutionItemType.PhysicalFolder => KnownMonikers.FolderOpened,
            SolutionItemType.VirtualFolder => KnownMonikers.FolderOpened,
            SolutionItemType.PhysicalFile => KnownMonikers.Document,
            _ => item is Project ? KnownMonikers.CSProjectNode : KnownMonikers.FolderOpened
        };
    }

    private static StackedImageMoniker GetFileMoniker(string filePath)
    {
        return (ImageMoniker?)FileNameToImageMonikerConverter.Convert(
            filePath,
            typeof(ImageMoniker),
            null,
            CultureInfo.CurrentCulture) ?? KnownMonikers.Document;
    }

    private static async Task OpenDocumentAsync(string filePath)
    {
        await VS.Documents.OpenAsync(filePath);
    }

    private static async Task ShowProjectMenuAsync(Project project, IWpfTextView currentTextView)
    {
        var projectInfo = await GenericProjectInfo.CreateFromProjectAsync(project);
        new LocationBreadcrumbMenuContext(projectInfo, currentTextView).ShowMenu();
    }
}
