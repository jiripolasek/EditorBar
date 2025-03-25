// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides helper methods to launch external editors and perform related actions.
/// </summary>
[SuppressMessage("ReSharper", "CatchAllClause")]
internal static class Launcher
{
    internal const string FileNamePlaceholderConstant = "$(FilePath)";

    /// <summary>
    /// Opens the specified file in an external editor.
    /// </summary>
    /// <param name="filePath">The path of the file to open.</param>
    internal static void OpenInExternalEditor(string? filePath)
    {
        if (StringHelper.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath) ||
            StringHelper.IsNullOrWhiteSpace(GeneralOptionsModel.Instance.ExternalEditorCommand))
        {
            return;
        }

        var command = GeneralOptionsModel.Instance.ExternalEditorCommand;

        var commandArgs = GeneralOptionsModel.Instance.ExternalEditorCommandArguments ?? "";

        // ensure file path as passed to the command arguments: if the arguments does not contain the placeholder, append it
        var hasPathPlaceholder =
            commandArgs.IndexOf(FileNamePlaceholderConstant, StringComparison.InvariantCultureIgnoreCase) > -1;
        if (!hasPathPlaceholder)
        {
            commandArgs += " " + Quote(FileNamePlaceholderConstant);
        }

        try
        {
            commandArgs = commandArgs.Replace(FileNamePlaceholderConstant, filePath);

            Process.Start(new ProcessStartInfo(command, commandArgs) { UseShellExecute = true });
            VS.StatusBar.ShowMessageAsync($"Opened {filePath} in external editor").FireAndForget();
        }
        catch (Exception ex)
        {
            ex.Log();
            VS.StatusBar.ShowMessageAsync($"Failed to open {filePath} in external editor").FireAndForget();
        }
    }

    /// <summary>
    /// Opens the specified file in the default editor.
    /// </summary>
    /// <param name="filePath">The path of the file to open.</param>
    internal static void OpenInDefaultEditor(string? filePath)
    {
        if (StringHelper.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            VS.StatusBar.ShowMessageAsync($"Opened {filePath} in default editor").FireAndForget();
        }
        catch (Exception ex)
        {
            ex.Log();
            VS.StatusBar.ShowMessageAsync($"Failed to open {filePath} in default editor").FireAndForget();
        }
    }

    /// <summary>
    /// Opens the containing folder of the specified file.
    /// </summary>
    /// <param name="filePath">The path of the file whose containing folder to open.</param>
    internal static void OpenContaingFolder(string? filePath)
    {
        if (StringHelper.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        var directoryName = Path.GetDirectoryName(filePath);
        if (StringHelper.IsNullOrWhiteSpace(directoryName!) || !Directory.Exists(directoryName))
        {
            return;
        }

        try
        {
            Process.Start(
                new ProcessStartInfo("explorer.exe", "/select, " + Quote(filePath)) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    /// <summary>
    /// Opens the specified directory in Windows Explorer.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to open.</param>
    internal static void OpenFolder(string? directoryPath)
    {
        if (StringHelper.IsNullOrWhiteSpace(directoryPath))
        {
            return;
        }

        if (StringHelper.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            return;
        }

        try
        {
            Process.Start(
                new ProcessStartInfo("explorer.exe", "/root, " + Quote(directoryPath)) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    /// <summary>
    /// Copies the absolute path of the specified file to the clipboard.
    /// </summary>
    /// <param name="filePath">The path of the file to copy.</param>
    internal static void CopyAbsolutePath(string? filePath)
    {
        try
        {
            if (!StringHelper.IsNullOrWhiteSpace(filePath))
            {
                Clipboard.SetText(filePath, TextDataFormat.UnicodeText);
            }

            VS.StatusBar.ShowMessageAsync("Full path copied to Clipboard").FireAndForget();
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    /// <summary>
    /// Copies the relative path from the solution to the specified file to the clipboard.
    /// </summary>
    /// <param name="filePath">The full path of the file.</param>
    internal static void CopyRelativePathFromFullPath(string? filePath)
    {
        try
        {
            var relativePath = GetRelativePathToSolution(filePath);
            if (!StringHelper.IsNullOrWhiteSpace(relativePath))
            {
                Clipboard.SetText(relativePath, TextDataFormat.UnicodeText);
            }

            VS.StatusBar.ShowMessageAsync("Relative path copied to Clipboard").FireAndForget();
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    private static string? GetRelativePathToSolution(string? path)
    {
        if (path == null)
        {
            return null;
        }

        var currentSolution = VS.Solutions.GetCurrentSolution();
        var slnPath = currentSolution?.FullPath;
        if (string.IsNullOrWhiteSpace(slnPath!))
        {
            return path;
        }

        var slnDir = Path.GetDirectoryName(slnPath!);
        return slnDir == null ? path : GetRelativePath(path, slnDir);

        static string GetRelativePath(string filePath, string slnDir)
        {
            return !filePath.StartsWith(slnDir, StringComparison.OrdinalIgnoreCase)
                ? filePath
                : filePath.Substring(slnDir.Length);
        }
    }


    private static string Quote(string fileName)
    {
        return $"""
                "{fileName}"
                """;
    }
}