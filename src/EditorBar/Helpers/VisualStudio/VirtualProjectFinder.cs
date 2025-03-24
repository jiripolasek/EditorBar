// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Helper class for finding files in virtual projects (e.g. "Miscellaneous Files").
/// </summary>
internal static class VirtualProjectFinder
{
    /// <summary>
    /// Searches all virtual projects (e.g. "Miscellaneous Files")
    /// for the specified file path, returning a <see cref="SolutionItem" /> if found.
    /// </summary>
    /// <param name="path">The file path to search for.</param>
    /// <exception cref="ArgumentException">Thrown when the provided path is null or whitespace.</exception>
    /// <returns>A <see cref="SolutionItem" /> if the file is found; otherwise, null.</returns>
    public static async Task<SolutionItem?> FindItemsInVirtualProjectsByPathAsync(string path)
    {
        Requires.NotNullOrWhiteSpace(path, nameof(path));

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        var vsSolution = await VS.GetServiceAsync<SVsSolution, IVsSolution>();
        if (vsSolution == null)
        {
            return null;
        }

        // Get enumerator for *virtual* projects
        // __VSENUMPROJFLAGS.EPF_VIRTUALPROJECTS includes "Miscellaneous Files".
        vsSolution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_ALLPROJECTS, Guid.Empty, out var enumHierarchies);
        if (enumHierarchies == null)
        {
            return null;
        }

        var hierarchy = new IVsHierarchy[1];
        while (true)
        {
            var result = enumHierarchies.Next(1, hierarchy, out var fetched);
            if (result != VSConstants.S_OK || fetched == 0)
            {
                break;
            }

            var candidate = hierarchy[0];
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (candidate != null)
            {
                // Try to find the file path inside this hierarchy
                var foundItemId = FindItemInHierarchy(candidate, path);
                if (foundItemId != VSConstants.VSITEMID_NIL)
                {
                    return await SolutionItem.FromHierarchyAsync(candidate, foundItemId);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Walks an IVsHierarchy to see if it contains a node whose canonical name matches <paramref name="path" />.
    /// Returns VSITEMID if found, or <see cref="VSConstants.VSITEMID_NIL" /> if not found.
    /// </summary>
    private static uint FindItemInHierarchy(IVsHierarchy hierarchy, string path)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        // We'll try ParseCanonicalName first, as many hierarchies support it:
        if (hierarchy.ParseCanonicalName(path, out var itemId) != VSConstants.S_OK)
        {
            return VSConstants.VSITEMID_NIL;
        }

        // If that didn't work, we could do a more manual walk over children, but 
        // typically "Misc Files" does implement ParseCanonicalName for open files. 
        return itemId;
    }
}