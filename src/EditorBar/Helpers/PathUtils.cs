// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.IO;
using MicrosoftPath = Microsoft.IO.Path;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides utility functions for handling file paths, including getting relative paths, checking if a path is a
/// network path, and comparing paths for equality.
/// </summary>
internal static class PathUtils
{
    /// <summary>
    /// Specifies the string comparison type used for local file paths.
    /// Uses case-insensitive comparison for compatibility with Windows file systems.
    /// </summary>
    public const StringComparison LocalPathComparison = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Specifies the string comparison type used for network file paths.
    /// Uses case-sensitive comparison for compatibility with network paths which may be case-sensitive.
    /// </summary>
    public const StringComparison NetworkPathComparison = StringComparison.Ordinal;

    /// <summary>
    /// Determines whether the specified path is a network path.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns>
    /// <c>true</c> if the path is a network path (UNC path starting with "\\" or a mapped network drive);
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNetworkPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        try
        {
            // Check if path is UNC (e.g., \\server\share\...)
            if (path.StartsWith(@"\\", NetworkPathComparison))
            {
                return true;
            }

            // Check if the path root is a network drive
            var driveInfo = new DriveInfo(MicrosoftPath.GetPathRoot(path)!);
            if (driveInfo.DriveType == DriveType.Network)
            {
                return true;
            }
        }
        catch
        {
            // Ignore exceptions; invalid paths will return false
        }

        return false;
    }

    /// <summary>
    /// Compares two file paths for equality, accounting for path normalization and network path handling.
    /// </summary>
    /// <param name="leftPath">The first path to compare.</param>
    /// <param name="rightPath">The second path to compare.</param>
    /// <returns>
    /// <c>true</c> if the paths refer to the same location; otherwise, <c>false</c>.
    /// Uses case-sensitive comparison for network paths and case-insensitive comparison for local paths.
    /// </returns>
    public static bool Equals(string? leftPath, string? rightPath)
    {
        if (leftPath == rightPath)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(leftPath!) || string.IsNullOrWhiteSpace(rightPath!))
        {
            return false;
        }

        leftPath = MicrosoftPath.GetFullPath(leftPath!);
        rightPath = MicrosoftPath.GetFullPath(rightPath!);

        var pathComparison = IsNetworkPath(leftPath) && IsNetworkPath(rightPath)
            ? NetworkPathComparison
            : LocalPathComparison;

        return string.Equals(leftPath, rightPath, pathComparison);
    }

    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="relativeTo" /> or <paramref name="path" /> is
    /// <c>null</c> or an empty string.
    /// </exception>
    internal static string GetRelativePath(string relativeTo, string path)
    {
        var result = MicrosoftPath.GetRelativePath(relativeTo, path);
        return result == "." ? "" : result;
    }
}