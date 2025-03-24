// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.IO;
using Community.VisualStudio.Toolkit;
using Microsoft;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides helper methods for interacting with project properties in Visual Studio.
/// </summary>
public static class ProjectProperties
{
    /// <summary>
    /// Selects the given <see cref="SolutionItem" /> in the Solution Explorer.
    /// </summary>
    /// <param name="item">The solution item to select (project, file, folder, etc.)</param>
    public static async Task SelectInSolutionExplorerAsync(this SolutionItem item)
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        var solutionExplorerWindow = await VS.Windows.GetSolutionExplorerWindowAsync();
        if (solutionExplorerWindow != null)
        {
            solutionExplorerWindow.SetSelection(item);
            solutionExplorerWindow.Frame.Show();
        }

        //await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        //// 1) Ensure that Solution Explorer is visible
        //IVsUIShell? uiShell = await VS.GetServiceAsync<SVsUIShell, IVsUIShell>();
        //if (uiShell is null)
        //    return;

        //Guid solutionExplorerGuid = new Guid(ToolWindowGuids80.SolutionExplorer);
        //uiShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref solutionExplorerGuid, out IVsWindowFrame? windowFrame);

        //// Show the window if found
        //windowFrame?.Show();

        //// 2) Retrieve the IVsUIHierarchyWindow from the Solution Explorer
        //if (windowFrame is null)
        //    return;

        //if (windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object docView) != VSConstants.S_OK)
        //    return;

        //if (docView is not IVsUIHierarchyWindow hierarchyWindow)
        //    return;

        //// 3) Get the IVsHierarchy and item ID from the SolutionItem
        //item.GetItemInfo(out IVsHierarchy vsHierarchy, out var itemId, out var hierarchyItem);
        //if (vsHierarchy is null)
        //    return;

        //// If for some reason you don't have an ItemId, you can attempt a fallback:
        //// vsHierarchy.ParseCanonicalName(item.FullPath, out vsItemId);

        //// 4) Expand/select the desired node
        //if (vsHierarchy is IVsUIHierarchy vsUIHierarchy)
        //{
        //    hierarchyWindow.ExpandItem(vsUIHierarchy, itemId, EXPANDFLAGS.EXPF_SelectItem);
        //}
    }

    /// <summary>
    /// Finds and selects the solution item (project, file, or folder) that matches the given physical path.
    /// </summary>
    /// <param name="path">A physical path to a file or folder in the solution.</param>
    public static async Task SelectInSolutionExplorerAsync(string path)
    {
        Requires.NotNullOrWhiteSpace(path, nameof(path));

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        // if path is a directory, ensure it ends with a backslash
        if (Directory.Exists(path) && !path.EndsWith("\\"))
        {
            path += "\\";
        }

        // 2) Find an item whose FullPath matches the specified path
        var targetItem = await FindItemByFullPathAsync(path);

        if (targetItem == null)
        {
            // The item might not be part of the solution or the path is incorrect
            // You can optionally show a message box or just return.
            // e.g., await VS.MessageBox.ShowWarningAsync("Not Found", $"No solution item found for path: {path}");
            return;
        }

        // 3) Select the found item in Solution Explorer
        await targetItem.SelectInSolutionExplorerAsync();
    }


    /// <summary>
    /// Recursively searches the solution for the first item whose FullPath
    /// matches the specified file or folder path. Returns null if not found.
    /// </summary>
    /// <param name="path">Physical file or folder path to match.</param>
    private static async Task<SolutionItem?> FindItemByFullPathAsync(string path)
    {
        Requires.NotNullOrWhiteSpace(path, nameof(path));

        // Ensure we're on the main thread for solution operations
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        // 1) Search in loaded projects
        {
            // Retrieve all top-level projects and solution folders
            var rootItems = await VS.Solutions.GetAllProjectsAsync();
            // Check each root item (project/folder) recursively
            foreach (var rootItem in rootItems)
            {
                var found = await FindItemRecursivelyAsync(rootItem, path);
                if (found != null)
                {
                    return found; // Return immediately on first match
                }
            }
        }

        // 2) Check unloaded project names
        {
            // Retrieve all unloaded projects
            var unloadedProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.Unloaded);
            var found = unloadedProjects.FirstOrDefault(unloadedProject =>
                unloadedProject.FullPath!.Equals(path, PathUtils.LocalPathComparison));
            if (found != null)
            {
                return found;
            }
        }

        // 3) Check miscellaneous items (e.g., Solution Items folder)
        {
            var found = await VirtualProjectFinder.FindItemsInVirtualProjectsByPathAsync(path);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    /// <summary>
    /// Depth-first search of a SolutionItem's children; stops on first match.
    /// </summary>
    private static async Task<SolutionItem?> FindItemRecursivelyAsync(SolutionItem item, string path)
    {
        // Check if this item matches the path
        if (!string.IsNullOrEmpty(item.FullPath!) && item.FullPath!.Equals(path, PathUtils.LocalPathComparison))
        {
            return item;
        }

        // Recursively check child items
        foreach (var child in item.Children)
        {
            if (child == null)
            {
                continue;
            }

            var found = await FindItemRecursivelyAsync(child, path);
            if (found != null)
            {
                return found;
            }
        }

        // Not found under this branch
        return null;
    }
}