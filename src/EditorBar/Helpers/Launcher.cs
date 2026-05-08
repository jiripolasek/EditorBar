// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides helper methods to launch external editors and perform related actions.
/// </summary>
[SuppressMessage("ReSharper", "CatchAllClause")]
internal static class Launcher
{
    internal const string FileNamePlaceholderConstant = "$(FilePath)";
    internal const string WorkingDirectoryPlaceholderConstant = "$(WorkingDirectory)";
    internal const string ItemPathPlaceholderConstant = "$(ItemPath)";
    internal const string DefaultTerminalCommand = "wt.exe";
    internal const string DefaultTerminalArguments = "-d \"$(WorkingDirectory)\"";

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

        var commandArgs = GeneralOptionsModel.Instance.ExternalEditorCommandArguments ?? string.Empty;

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
    /// Opens a terminal rooted at the specified file or directory path.
    /// </summary>
    /// <param name="itemPath">The file or directory path the action was invoked on.</param>
    internal static void OpenTerminal(string? itemPath)
    {
        var terminalProfile = NormalizeSupportedTerminalProfile(GeneralOptionsModel.Instance.TerminalProfile);

        if (!TryResolveWorkingDirectory(itemPath, out var resolvedItemPath, out var workingDirectory))
        {
            VS.StatusBar.ShowMessageAsync("Failed to resolve terminal working directory").FireAndForget();
            return;
        }

        var configuredCommand = (GeneralOptionsModel.Instance.TerminalCommand ?? string.Empty).Trim();
        var configuredArguments = (GeneralOptionsModel.Instance.TerminalCommandArguments ?? string.Empty).Trim();

        var command = terminalProfile == TerminalProfile.Custom
            ? configuredCommand
            : GetTerminalCommand(terminalProfile);
        var arguments = terminalProfile == TerminalProfile.Custom
            ? configuredArguments
            : GetTerminalArguments(terminalProfile, workingDirectory);
        var useDefaultTerminal = terminalProfile == TerminalProfile.WindowsTerminal;

        if (StringHelper.IsNullOrWhiteSpace(command) || arguments == null)
        {
            VS.StatusBar.ShowMessageAsync($"Failed to open terminal at {workingDirectory}").FireAndForget();
            return;
        }

        if (TryStartTerminal(command, arguments, resolvedItemPath, workingDirectory))
        {
            VS.StatusBar.ShowMessageAsync($"Opened terminal at {workingDirectory}").FireAndForget();
            return;
        }

        if (!useDefaultTerminal)
        {
            VS.StatusBar.ShowMessageAsync($"Failed to open terminal at {workingDirectory}").FireAndForget();
            return;
        }

        if (TryStartTerminal("cmd.exe", string.Empty, resolvedItemPath, workingDirectory))
        {
            VS.StatusBar.ShowMessageAsync($"Opened terminal at {workingDirectory} using cmd.exe").FireAndForget();
            return;
        }

        VS.StatusBar.ShowMessageAsync($"Failed to open terminal at {workingDirectory}").FireAndForget();
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

    internal static bool IsDefaultTerminalConfiguration(string? configuredCommand, string? configuredArguments)
    {
        return string.IsNullOrWhiteSpace(configuredCommand) ||
               (string.Equals(configuredCommand, DefaultTerminalCommand, StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(configuredArguments) ||
                 string.Equals(configuredArguments, DefaultTerminalArguments, StringComparison.Ordinal)));
    }

    internal static TerminalProfile NormalizeSupportedTerminalProfile(TerminalProfile terminalProfile)
    {
        return terminalProfile == TerminalProfile.VisualStudioIntegratedTerminal
            ? TerminalProfile.WindowsTerminal
            : terminalProfile;
    }

    internal static string GetTerminalDisplayCommand(TerminalProfile terminalProfile)
    {
        terminalProfile = NormalizeSupportedTerminalProfile(terminalProfile);
        return terminalProfile switch
        {
            TerminalProfile.WindowsTerminal => DefaultTerminalCommand,
            TerminalProfile.CommandPrompt => "cmd.exe",
            TerminalProfile.WindowsPowerShell => "powershell.exe",
            TerminalProfile.PowerShell => "pwsh.exe",
            TerminalProfile.DeveloperPowerShell => "powershell.exe",
            TerminalProfile.Custom => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(terminalProfile))
        };
    }

    internal static string GetTerminalDisplayArguments(TerminalProfile terminalProfile)
    {
        terminalProfile = NormalizeSupportedTerminalProfile(terminalProfile);
        return terminalProfile switch
        {
            TerminalProfile.WindowsTerminal => DefaultTerminalArguments,
            TerminalProfile.CommandPrompt => string.Empty,
            TerminalProfile.WindowsPowerShell => "-NoExit",
            TerminalProfile.PowerShell => "-NoExit",
            TerminalProfile.DeveloperPowerShell =>
                "-NoExit -ExecutionPolicy Bypass -Command \"& '<Launch-VsDevShell.ps1>'; Set-Location -LiteralPath '$(WorkingDirectory)'\"",
            TerminalProfile.Custom => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(terminalProfile))
        };
    }

    internal static string GetTerminalPresetNote(TerminalProfile terminalProfile)
    {
        terminalProfile = NormalizeSupportedTerminalProfile(terminalProfile);
        return terminalProfile switch
        {
            TerminalProfile.WindowsTerminal =>
                "Uses Windows Terminal and falls back to cmd.exe if Windows Terminal is unavailable.",
            TerminalProfile.CommandPrompt =>
                "Starts cmd.exe in the resolved working directory.",
            TerminalProfile.WindowsPowerShell =>
                "Starts Windows PowerShell and keeps the shell open in the resolved working directory.",
            TerminalProfile.PowerShell =>
                "Starts PowerShell (pwsh) and keeps the shell open in the resolved working directory.",
            TerminalProfile.DeveloperPowerShell =>
                "Starts Developer PowerShell for the current Visual Studio installation, then switches to the resolved working directory.",
            TerminalProfile.Custom =>
                "Use custom executable and arguments. $(WorkingDirectory) and $(ItemPath) placeholders are supported.",
            _ => throw new ArgumentOutOfRangeException(nameof(terminalProfile))
        };
    }

    private static bool TryResolveWorkingDirectory(
        string? itemPath,
        [NotNullWhen(true)] out string? resolvedItemPath,
        [NotNullWhen(true)] out string? workingDirectory)
    {
        resolvedItemPath = null;
        workingDirectory = null;

        if (StringHelper.IsNullOrWhiteSpace(itemPath))
        {
            return false;
        }

        resolvedItemPath = itemPath.Trim();

        if (Directory.Exists(resolvedItemPath))
        {
            workingDirectory = resolvedItemPath;
            return true;
        }

        var directoryName = Path.GetDirectoryName(resolvedItemPath);
        if (!StringHelper.IsNullOrWhiteSpace(directoryName!) && Directory.Exists(directoryName))
        {
            workingDirectory = directoryName;
            return true;
        }

        return false;
    }

    private static bool TryStartTerminal(
        string command,
        string arguments,
        string itemPath,
        string workingDirectory)
    {
        try
        {
            var expandedArguments = ExpandTerminalArguments(arguments, itemPath, workingDirectory);
            Process.Start(
                new ProcessStartInfo(command)
                {
                    Arguments = expandedArguments,
                    UseShellExecute = true,
                    WorkingDirectory = workingDirectory
                });
            return true;
        }
        catch (Win32Exception ex)
        {
            ex.Log();
            return false;
        }
        catch (FileNotFoundException ex)
        {
            ex.Log();
            return false;
        }
        catch (Exception ex)
        {
            ex.Log();
            return false;
        }
    }

    private static string ExpandTerminalArguments(string arguments, string itemPath, string workingDirectory)
    {
        return arguments
            .Replace(WorkingDirectoryPlaceholderConstant, workingDirectory)
            .Replace(ItemPathPlaceholderConstant, itemPath);
    }

    private static string? GetTerminalCommand(TerminalProfile terminalProfile)
    {
        terminalProfile = NormalizeSupportedTerminalProfile(terminalProfile);
        return terminalProfile switch
        {
            TerminalProfile.WindowsTerminal => DefaultTerminalCommand,
            TerminalProfile.CommandPrompt => "cmd.exe",
            TerminalProfile.WindowsPowerShell => "powershell.exe",
            TerminalProfile.PowerShell => "pwsh.exe",
            TerminalProfile.DeveloperPowerShell => "powershell.exe",
            TerminalProfile.Custom => (GeneralOptionsModel.Instance.TerminalCommand ?? string.Empty).Trim(),
            _ => null
        };
    }

    private static string? GetTerminalArguments(TerminalProfile terminalProfile, string workingDirectory)
    {
        terminalProfile = NormalizeSupportedTerminalProfile(terminalProfile);
        return terminalProfile switch
        {
            TerminalProfile.WindowsTerminal => DefaultTerminalArguments,
            TerminalProfile.CommandPrompt => string.Empty,
            TerminalProfile.WindowsPowerShell => "-NoExit",
            TerminalProfile.PowerShell => "-NoExit",
            TerminalProfile.DeveloperPowerShell => GetDeveloperPowerShellArguments(workingDirectory),
            TerminalProfile.Custom => (GeneralOptionsModel.Instance.TerminalCommandArguments ?? string.Empty).Trim(),
            _ => null
        };
    }

    private static string? GetDeveloperPowerShellArguments(string workingDirectory)
    {
        var launchScriptPath = GetDeveloperPowerShellScriptPath();
        if (StringHelper.IsNullOrWhiteSpace(launchScriptPath))
        {
            return null;
        }

        return $"-NoExit -ExecutionPolicy Bypass -Command \"& '{EscapePowerShellSingleQuotedString(launchScriptPath)}'; Set-Location -LiteralPath '{EscapePowerShellSingleQuotedString(workingDirectory)}'\"";
    }

    private static string? GetDeveloperPowerShellScriptPath()
    {
        var devEnvDir = Environment.GetEnvironmentVariable("DevEnvDir");
        if (StringHelper.IsNullOrWhiteSpace(devEnvDir))
        {
            devEnvDir = Environment.GetEnvironmentVariable("VSAPPIDDIR");
        }

        if (StringHelper.IsNullOrWhiteSpace(devEnvDir))
        {
            return null;
        }

        var ideDirectory = devEnvDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var common7Directory = Directory.GetParent(ideDirectory)?.FullName;
        if (StringHelper.IsNullOrWhiteSpace(common7Directory))
        {
            return null;
        }

        var launchScriptPath = Path.Combine(common7Directory, "Tools", "Launch-VsDevShell.ps1");
        return File.Exists(launchScriptPath) ? launchScriptPath : null;
    }

    private static string EscapePowerShellSingleQuotedString(string value)
    {
        return value.Replace("'", "''");
    }

}
