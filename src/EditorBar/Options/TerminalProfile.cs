// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Defines predefined terminal and shell launch profiles.
/// </summary>
public enum TerminalProfile
{
    [Description("Windows Terminal")]
    WindowsTerminal,

    [Description("Command Prompt")]
    CommandPrompt,

    [Description("Windows PowerShell")]
    WindowsPowerShell,

    [Description("PowerShell (pwsh)")]
    PowerShell,

    [Description("Developer PowerShell")]
    DeveloperPowerShell,

    [Description("Visual Studio integrated terminal")]
    VisualStudioIntegratedTerminal,

    [Description("Custom")]
    Custom
}
