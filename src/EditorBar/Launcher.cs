// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar;

[SuppressMessage("ReSharper", "CatchAllClause")]
internal static class Launcher
{
    internal const string FileNamePlaceholderConstant = "$(FilePath)";

    internal static void OpenInExternalEditor(string? filePath)
    {
        if (StringHelper.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath) || StringHelper.IsNullOrWhiteSpace(GeneralPage.Instance.ExternalEditorCommand))
        {
            return;
        }

        var command = GeneralPage.Instance.ExternalEditorCommand;

        var commandArgs = (GeneralPage.Instance.ExternalEditorCommandArguments ?? "");

        // ensure file path as passed to the command arguments: if the arguments does not contain the placeholder, append it
        var hasPathPlaceholder = commandArgs.IndexOf(FileNamePlaceholderConstant, StringComparison.InvariantCultureIgnoreCase) > -1;
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
            Process.Start(new ProcessStartInfo("explorer.exe", "/select, " + Quote(filePath)) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

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

    internal static void CopyRelativePath(string? relativePath)
    {
        try
        {
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

    private static string Quote(string fileName)
    {
        return $"""
                "{fileName}"
                """;
    }
}