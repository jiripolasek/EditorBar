// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

#pragma warning disable IDE0079 // Remove unnecessary suppression (IDE0079)

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Model specifies general options for the Editor Bar.
/// </summary>
/// <seealso cref="BaseOptionModel{GeneralPage}" />
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used implicitly.")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Setters are used implicitly by PropertyGrid.")]
[ComVisible(true)]
public class GeneralOptionsModel : BaseOptionModel<GeneralOptionsModel>, IRatingConfig
{
    private const int CurrentConfigVersion = 8;

    private const string AppearanceCategoryName = "Appearance";
    private const string GeneralCategoryName = "General";
    private const string ColorsCategoryName = "Colors";
    private const string AdditionalActionCategoryName = "Actions";
    private const string MemberListCategoryName = "Member List";
    private const string ToolbarCategoryName = "Toolbar";
    private const string ExternalEditorCategoryName = "External Editor";
    private const string TerminalCategoryName = "Terminal";

    private const string RegistryCollectionName = "JPSoftworks.EditorBar.Options.GeneralPage";
    internal const string PathToEnabledProperty = RegistryCollectionName + @"\" + nameof(Enabled);

    // keep legacy name to  keep settings for existing users intact

    /// <inheritdoc />
    protected override string CollectionName => RegistryCollectionName;

    // -------------------------------------------
    // Appearance category
    // -------------------------------------------
    [Category(AppearanceCategoryName)]
    [DisplayName("Bar position")]
    [Description("Position of the Editor Bar.")]
    [DefaultValue(BarPosition.Top)]
    [TypeConverter(typeof(EnumConverter))]
    public BarPosition BarPosition { get; set; } = BarPosition.Top;

    [Category(AppearanceCategoryName)]
    [DisplayName("Show relative path")]
    [Description("Show path relative to the solution root.")]
    [DefaultValue(true)]
    public bool ShowPathRelativeToSolutionRoot { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("File name display style")]
    [Description("Show path relative to the solution root.")]
    [DefaultValue(FileLabel.FileName)]
    public FileLabel FileLabelStyle { get; set; } = FileLabel.Hidden;

    [Category(AppearanceCategoryName)]
    [DisplayName("Show solution folders")]
    [Description("Show solution folder block elements in the Editor Bar.")]
    [DefaultValue(true)]
    public bool ShowSolutionFolders { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("Show project folders")]
    [Description("Show project folders in the Editor Bar.")]
    [DefaultValue(true)]
    public bool ShowProjectFolders { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("Show parent folder")]
    [Description("Show immediate parent folder element in the Editor Bar.")]
    [DefaultValue(true)]
    public bool ShowParentFolder { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("Show solution root")]
    [Description("Show solution root element in the Editor Bar.")]
    [DefaultValue(true)]
    public bool ShowSolutionRoot { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("Show project")]
    [Description("Show project that current documents belongs to in the Editor Bar.")]
    [DefaultValue(true)]
    public bool ShowProject { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("Display mode")]
    [Description(
        "Choose the style of the Editor Bar. Normal mode is more specious, compact mode gives you more vertical space for you code.")]
    [DefaultValue(DisplayStyle.Normal)]
    public DisplayStyle DisplayStyle { get; set; } = DisplayStyle.Normal;

    [Category(AppearanceCategoryName)]
    [DisplayName("Visual Style")]
    [Description("Choose the theme of the Editor Bar.")]
    [DefaultValue(VisualStyle.FullRowTransparent)]
    public VisualStyle VisualStyle { get; set; } = VisualStyle.FullRowTransparent;

    // show structural breadcrumbs
    [Category(AppearanceCategoryName)]
    [DisplayName("Show code structure breadcrumbs")]
    [Description("Show breadcrumbs for code structure elements like classes, methods, etc.")]
    [DefaultValue(true)]
    public bool ShowCodeStructureBreadcrumbs { get; set; } = true;

    // show structural breadcrumbs
    [Category(AppearanceCategoryName)]
    [DisplayName("Show file name breadcrumb")]
    [Description("Show file name breadcrumb.")]
    [DefaultValue(true)]
    public bool ShowFileNameBreadcrumb { get; set; } = true;

    // -------------------------------------------
    // General category
    // -------------------------------------------
    [Category(GeneralCategoryName)]
    [DisplayName("Enable Editor Bar")]
    [Description("Determines if the Editor Bar is visible.")]
    [DefaultValue(true)]
    public bool Enabled { get; set; } = true;

    [Category(ToolbarCategoryName)]
    [DisplayName("Show locate in Solution Explorer button")]
    [Description("Determines if the Locate in Solution Explorer button is visible on the Editor Bar toolbar.")]
    [DefaultValue(true)]
    public bool ShowLocateInSolutionExplorerButton { get; set; } = true;

    [Category(ToolbarCategoryName)]
    [DisplayName("Show open in default editor button")]
    [Description("Determines if the Open in Default Editor button is visible on the Editor Bar toolbar.")]
    [DefaultValue(true)]
    public bool ShowOpenDefaultEditorButton { get; set; } = true;

    [Category(ToolbarCategoryName)]
    [DisplayName("Show open in external editor button")]
    [Description("Determines if the Open in External Editor button is visible on the Editor Bar toolbar.")]
    [DefaultValue(true)]
    public bool ShowOpenExternalEditorButton { get; set; } = true;

    [Category(ToolbarCategoryName)]
    [DisplayName("Show open containing folder button")]
    [Description("Determines if the Open Containing Folder button is visible on the Editor Bar toolbar.")]
    [DefaultValue(true)]
    public bool ShowOpenContainingFolderButton { get; set; } = true;

    [Category(ToolbarCategoryName)]
    [DisplayName("Show open in terminal button")]
    [Description("Determines if the Open in Terminal button is visible on the Editor Bar toolbar.")]
    [DefaultValue(true)]
    public bool ShowOpenTerminalButton { get; set; } = true;

    // -------------------------------------------
    // External Editor category
    // -------------------------------------------
    [Category(ExternalEditorCategoryName)]
    [DisplayName("External editor executable")]
    [Description("Path to external editor or command.")]
    [DefaultValue("notepad.exe")]
    public string? ExternalEditorCommand { get; set; } = "notepad.exe";

    [Category(ExternalEditorCategoryName)]
    [DisplayName("External editor executable arguments")]
    [Description(
        "Path to external editor or command. " + Launcher.FileNamePlaceholderConstant +
        " represents the file name.")]
    [DefaultValue(Launcher.FileNamePlaceholderConstant)]
    public string? ExternalEditorCommandArguments { get; set; } = Launcher.FileNamePlaceholderConstant;

    // -------------------------------------------
    // Terminal category
    // -------------------------------------------
    [Category(TerminalCategoryName)]
    [DisplayName("Terminal preset")]
    [Description("Predefined terminal or shell configuration.")]
    [DefaultValue(typeof(TerminalProfile), nameof(TerminalProfile.WindowsTerminal))]
    [TypeConverter(typeof(EnumToDescriptionConverter))]
    public TerminalProfile TerminalProfile { get; set; } = TerminalProfile.WindowsTerminal;

    [Category(TerminalCategoryName)]
    [DisplayName("Custom terminal executable")]
    [Description("Path to custom terminal executable or command.")]
    [DefaultValue(Launcher.DefaultTerminalCommand)]
    public string? TerminalCommand { get; set; } = Launcher.DefaultTerminalCommand;

    [Category(TerminalCategoryName)]
    [DisplayName("Custom terminal executable arguments")]
    [Description(
        "Arguments passed to the custom terminal executable. " + Launcher.WorkingDirectoryPlaceholderConstant +
        " represents the working directory and " + Launcher.ItemPathPlaceholderConstant +
        " represents the invoked file or folder path.")]
    [DefaultValue(Launcher.DefaultTerminalArguments)]
    public string? TerminalCommandArguments { get; set; } = Launcher.DefaultTerminalArguments;

    // -------------------------------------------
    // Activation rules category
    // -------------------------------------------
    [Category(GeneralCategoryName)]
    [DisplayName("Display in diff views")]
    [Description("Determines if the Editor Bar is visible in diff views.")]
    [DefaultValue(false)]
    public bool DisplayInDiffViews { get; set; }

    [Category(GeneralCategoryName)]
    [DisplayName("Display in auxiliary documents")]
    [Description("Determines if the Editor Bar is visible in auxiliary documents.")]
    [DefaultValue(false)]
    public bool DisplayInAuxiliaryDocuments { get; set; }

    [Category(GeneralCategoryName)]
    [DisplayName("Display in read-only documents")]
    [Description("Determines if the Editor Bar is visible in read-only documents.")]
    [DefaultValue(false)]
    public bool DisplayInNonEditableDocuments { get; set; }

    // display in blam
    [Category(GeneralCategoryName)]
    [DisplayName("Display in Blame")]
    [Description("Determines if the Editor Bar is visible in annotations / blame view.")]
    [DefaultValue(false)]
    public bool DisplayInBlame { get; set; }

    // display in temp files
    [Category(GeneralCategoryName)]
    [DisplayName("Display in temp files")]
    [Description("Determines if the Editor Bar is visible in temp files.")]
    [DefaultValue(false)]
    public bool DisplayInTempFiles { get; set; }

    // -------------------------------------------
    // Additional Actions category
    // -------------------------------------------
    [Category(AdditionalActionCategoryName)]
    [DisplayName("Double-click action")]
    [Description("Action to be performed when double-clicking on the file path.")]
    [DefaultValue(typeof(FileAction), nameof(FileAction.OpenContainingFolder))]
    [TypeConverter(typeof(EnumToDescriptionConverter))]
    public FileAction FileAction { get; set; } = FileAction.OpenContainingFolder;

    [Category(AdditionalActionCategoryName)]
    [DisplayName("Double-click + CTRL action")]
    [Description("Action to be performed when double-clicking on the file path.")]
    [DefaultValue(typeof(FileAction), nameof(FileAction.OpenInExternalEditor))]
    [TypeConverter(typeof(EnumToDescriptionConverter))]
    public FileAction AlternateFileAction { get; set; } = FileAction.OpenInExternalEditor;

    [Category(MemberListCategoryName)]
    [DisplayName("Show filter box when empty")]
    [Description("Determines if the member list filter box stays visible even when it does not contain any text.")]
    [DefaultValue(false)]
    public bool ShowMemberListFilterBoxWhenEmpty { get; set; } = false;

    // -------------------------------------------
    // Debug category
    // -------------------------------------------
    [Category("Debug")]
    [DisplayName("Debug mode")]
    [Description("Enable debug mode.")]
    [DefaultValue(false)]
    public bool DebugMode { get; set; }

    // -------------------------------------------
    // Meta
    // -------------------------------------------
    [Browsable(false)]
    public int Version { get; set; }

    [Browsable(false)]
    public string? VsixVersion { get; set; }

    [Browsable(false)]
    public int RatingRequests { get; set; }

    private static bool EqualColor(Color left, Color right)
    {
        return left.ToArgb() == right.ToArgb();
    }

    private static Color ResolveLegacyLightColor(Color currentColor, Color darkDefaultColor, Color legacyLightDefaultColor)
    {
        return EqualColor(currentColor, darkDefaultColor) ? legacyLightDefaultColor : currentColor;
    }

    // -------------------------------------------
    // Methods
    // -------------------------------------------
    public async Task UpgradeAsync()
    {
        var changed = false;

        // marked last used extension version; if changed, we can show What's new dialog, etc.
        if (this.VsixVersion != Vsix.Version)
        {
            this.VsixVersion = Vsix.Version;
            changed = true;
        }

        // check last used config version and upgrade if necessary
        if (this.Version < CurrentConfigVersion)
        {
            // Sequential upgrade across config versions
            if (this.Version < 2)
            {
                // When upgrading from version 1 to 2, we need to change the default value of FileLabelStyle
                // If the user had relative paths enabled, then we set the FileLabelStyle to FileName (not relative), because with this update
                // we are also adding breadcrumbs for in-project folders and parent folder that supersedes the need for relative paths.
                // User can disable these new features and revert to relative paths if they want manually.
                //
                // For absolute path, let's just keep the setting as it is. User might be annoyed by the long paths, which may force them to
                // go to settings. This should be "fixed" later by adding What's new dialog.
                this.FileLabelStyle = this.ShowPathRelativeToSolutionRoot ? FileLabel.FileName : FileLabel.AbsolutePath;
            }

            if (this.Version < 3)
            {
                // When upgrading from version 2 to 3, we change value of FileLabelStyle.
                // If the user had FileLabelStyle set to FileName, we now change it to hidden, because its functions
                // will be covered by the new file name breadcrumb.
                if (this.FileLabelStyle == FileLabel.FileName)
                {
                    this.FileLabelStyle = FileLabel.Hidden;
                }
            }

            if (this.Version < 4)
            {
                if (StringHelper.IsNullOrWhiteSpace(this.TerminalCommand))
                {
                    this.TerminalCommand = Launcher.DefaultTerminalCommand;
                }

                if (StringHelper.IsNullOrWhiteSpace(this.TerminalCommandArguments))
                {
                    this.TerminalCommandArguments = Launcher.DefaultTerminalArguments;
                }
            }

            if (this.Version < 5)
            {
                this.TerminalProfile = Launcher.IsDefaultTerminalConfiguration(
                    this.TerminalCommand,
                    this.TerminalCommandArguments)
                    ? TerminalProfile.WindowsTerminal
                    : TerminalProfile.Custom;
            }

            if (this.Version < 6)
            {
                this.ShowMemberListFilterBoxWhenEmpty = false;
            }

            if (this.Version < 7)
            {
                this.LightSolutionBackground = ResolveLegacyLightColor(
                    this.SolutionBackground,
                    DarkSolutionBackgroundDefault,
                    LegacySolutionBackgroundDefault);
                this.LightSolutionForeground = ResolveLegacyLightColor(
                    this.SolutionForeground,
                    DarkSolutionForegroundDefault,
                    LegacySolutionForegroundDefault);
                this.LightNonSolutionRootBackground = ResolveLegacyLightColor(
                    this.NonSolutionRootBackground,
                    DarkNonSolutionRootBackgroundDefault,
                    LegacyNonSolutionRootBackgroundDefault);
                this.LightNonSolutionRootForeground = ResolveLegacyLightColor(
                    this.NonSolutionRootForeground,
                    DarkNonSolutionRootForegroundDefault,
                    LegacyNonSolutionRootForegroundDefault);
                this.LightProjectBackground = ResolveLegacyLightColor(
                    this.ProjectBackground,
                    DarkProjectBackgroundDefault,
                    LegacyProjectBackgroundDefault);
                this.LightProjectForeground = ResolveLegacyLightColor(
                    this.ProjectForeground,
                    DarkProjectForegroundDefault,
                    LegacyProjectForegroundDefault);
                this.LightSolutionFolderBackground = ResolveLegacyLightColor(
                    this.SolutionFolderBackground,
                    DarkSolutionFolderBackgroundDefault,
                    LegacySolutionFolderBackgroundDefault);
                this.LightSolutionFolderForeground = ResolveLegacyLightColor(
                    this.SolutionFolderForeground,
                    DarkSolutionFolderForegroundDefault,
                    LegacySolutionFolderForegroundDefault);
                this.LightParentFolderBackground = ResolveLegacyLightColor(
                    this.ParentFolderBackground,
                    DarkParentFolderBackgroundDefault,
                    LegacyParentFolderBackgroundDefault);
                this.LightParentFolderForeground = ResolveLegacyLightColor(
                    this.ParentFolderForeground,
                    DarkParentFolderForegroundDefault,
                    LegacyParentFolderForegroundDefault);
                this.LightProjectFoldersBackground = ResolveLegacyLightColor(
                    this.ProjectFoldersBackground,
                    DarkProjectFoldersBackgroundDefault,
                    LegacyProjectFoldersBackgroundDefault);
                this.LightProjectFoldersForeground = ResolveLegacyLightColor(
                    this.ProjectFoldersForeground,
                    DarkProjectFoldersForegroundDefault,
                    LegacyProjectFoldersForegroundDefault);
                this.LightFileBreadcrumbBackground = ResolveLegacyLightColor(
                    this.FileBreadcrumbBackground,
                    DarkFileBreadcrumbBackgroundDefault,
                    LegacyFileBreadcrumbBackgroundDefault);
                this.LightFileBreadcrumbForeground = ResolveLegacyLightColor(
                    this.FileBreadcrumbForeground,
                    DarkFileBreadcrumbForegroundDefault,
                    LegacyFileBreadcrumbForegroundDefault);
                this.LightStructureBreadcrumbBackground = ResolveLegacyLightColor(
                    this.StructureBreadcrumbBackground,
                    DarkStructureBreadcrumbBackgroundDefault,
                    LegacyStructureBreadcrumbBackgroundDefault);
                this.LightStructureBreadcrumbForeground = ResolveLegacyLightColor(
                    this.StructureBreadcrumbForeground,
                    DarkStructureBreadcrumbForegroundDefault,
                    LegacyStructureBreadcrumbForegroundDefault);
            }

            if (this.Version < 8 && this.HasLegacySharedDarkPalette())
            {
                this.ApplyDarkThemeDefaults();
            }

            this.Version = CurrentConfigVersion;
            changed = true;
        }

        if (changed)
        {
            await this.SaveAsync();
        }
    }

    // -------------------------------------------
    // Colors category
    // -------------------------------------------
    // The original color properties below are kept as the dark appearance color set for backward compatibility.
    // Separate light appearance colors are stored in the Light* properties further below.

    // Comparison of System.Drawing.Colors is little bit funny: Color.Black != Color.FromArgb(0, 0, 0)
    // So let's do comparison by ARGB values. See ColorEquality method below.
    #region Solution Background

    private static readonly Color LegacySolutionBackgroundDefault = Color.Purple;
    private static readonly Color DarkSolutionBackgroundDefault = Color.FromArgb(92, 58, 145);

    [Category(ColorsCategoryName)]
    [DisplayName("Solution root background color")]
    [Description("Background color of solution element.")]
    public Color SolutionBackground { get; set; } = DarkSolutionBackgroundDefault;

    public bool ShouldSerializeSolutionBackground()
    {
        return !EqualColor(this.SolutionBackground, DarkSolutionBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionBackground()
    {
        this.SolutionBackground = DarkSolutionBackgroundDefault;
    }

    #endregion

    #region Solution Foreground

    private static readonly Color LegacySolutionForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkSolutionForegroundDefault = Color.WhiteSmoke;

    [Category(ColorsCategoryName)]
    [DisplayName("Solution root text Color")]
    [Description("Foreground color of solution element.")]
    public Color SolutionForeground { get; set; } = DarkSolutionForegroundDefault;

    public bool ShouldSerializeSolutionForeground()
    {
        return !EqualColor(this.SolutionForeground, DarkSolutionForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionForeground()
    {
        this.SolutionForeground = DarkSolutionForegroundDefault;
    }

    #endregion

    #region NonSolutionRoot Background

    private static readonly Color LegacyNonSolutionRootBackgroundDefault = Color.Silver;
    private static readonly Color DarkNonSolutionRootBackgroundDefault = Color.FromArgb(96, 96, 96);

    [Category(ColorsCategoryName)]
    [DisplayName("NonSolutionRoot root background color")]
    [Description("Background color of NonSolutionRoot element.")]
    public Color NonSolutionRootBackground { get; set; } = DarkNonSolutionRootBackgroundDefault;

    public bool ShouldSerializeNonSolutionRootBackground()
    {
        return !EqualColor(this.NonSolutionRootBackground, DarkNonSolutionRootBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeNonSolutionRootBackground()
    {
        this.NonSolutionRootBackground = DarkNonSolutionRootBackgroundDefault;
    }

    #endregion

    #region NonSolutionRoot Foreground

    private static readonly Color LegacyNonSolutionRootForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkNonSolutionRootForegroundDefault = Color.WhiteSmoke;

    [Category(ColorsCategoryName)]
    [DisplayName("NonSolutionRoot root text Color")]
    [Description("Foreground color of NonSolutionRoot element.")]
    public Color NonSolutionRootForeground { get; set; } = DarkNonSolutionRootForegroundDefault;

    public bool ShouldSerializeNonSolutionRootForeground()
    {
        return !EqualColor(this.NonSolutionRootForeground, DarkNonSolutionRootForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeNonSolutionRootForeground()
    {
        this.NonSolutionRootForeground = DarkNonSolutionRootForegroundDefault;
    }

    #endregion

    #region Project Background

    private static readonly Color LegacyProjectBackgroundDefault = Color.LightSkyBlue;
    private static readonly Color DarkProjectBackgroundDefault = Color.FromArgb(48, 111, 160);

    [Category(ColorsCategoryName)]
    [DisplayName("Project background color")]
    [Description("Background color of project name element.")]
    public Color ProjectBackground { get; set; } = DarkProjectBackgroundDefault;

    public bool ShouldSerializeProjectBackground()
    {
        return !EqualColor(this.ProjectBackground, DarkProjectBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectBackground()
    {
        this.ProjectBackground = DarkProjectBackgroundDefault;
    }

    #endregion

    #region Project Foreground

    private static readonly Color LegacyProjectForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkProjectForegroundDefault = Color.WhiteSmoke;

    [Category(ColorsCategoryName)]
    [DisplayName("Project text color")]
    [Description("Foreground color of project name.")]
    public Color ProjectForeground { get; set; } = DarkProjectForegroundDefault;

    public bool ShouldSerializeProjectForeground()
    {
        return !EqualColor(this.ProjectForeground, DarkProjectForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectForeground()
    {
        this.ProjectForeground = DarkProjectForegroundDefault;
    }

    #endregion

    #region Solution Folder Background

    private static readonly Color LegacySolutionFolderBackgroundDefault = Color.Gold;
    private static readonly Color DarkSolutionFolderBackgroundDefault = Color.FromArgb(173, 131, 40);

    [Category(ColorsCategoryName)]
    [DisplayName("Solution folder background color")]
    [Description("Background color of solution folder element.")]
    public Color SolutionFolderBackground { get; set; } = DarkSolutionFolderBackgroundDefault;

    public bool ShouldSerializeSolutionFolderBackground()
    {
        return !EqualColor(this.SolutionFolderBackground, DarkSolutionFolderBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionFolderBackground()
    {
        this.SolutionFolderBackground = DarkSolutionFolderBackgroundDefault;
    }

    #endregion

    #region Solution Folder Foreground

    private static readonly Color LegacySolutionFolderForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkSolutionFolderForegroundDefault = Color.WhiteSmoke;

    [Category(ColorsCategoryName)]
    [DisplayName("Solution folder text color")]
    [Description("Foreground color of solution folder element.")]
    public Color SolutionFolderForeground { get; set; } = DarkSolutionFolderForegroundDefault;

    public bool ShouldSerializeSolutionFolderForeground()
    {
        return !EqualColor(this.SolutionFolderForeground, DarkSolutionFolderForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionFolderForeground()
    {
        this.SolutionFolderForeground = DarkSolutionFolderForegroundDefault;
    }

    #endregion

    #region Parent Folder Background

    private static readonly Color LegacyParentFolderBackgroundDefault = Color.YellowGreen;
    private static readonly Color DarkParentFolderBackgroundDefault = Color.FromArgb(102, 134, 57);

    [Category(ColorsCategoryName)]
    [DisplayName("Parent folder background color")]
    [Description("Background color of Parent folder element.")]
    public Color ParentFolderBackground { get; set; } = DarkParentFolderBackgroundDefault;

    public bool ShouldSerializeParentFolderBackground()
    {
        return !EqualColor(this.ParentFolderBackground, DarkParentFolderBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeParentFolderBackground()
    {
        this.ParentFolderBackground = DarkParentFolderBackgroundDefault;
    }

    #endregion

    #region Parent Folder Foreground

    private static readonly Color LegacyParentFolderForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkParentFolderForegroundDefault = Color.WhiteSmoke;

    [Category(ColorsCategoryName)]
    [DisplayName("Parent folder text color")]
    [Description("Foreground color of Parent folder element.")]
    public Color ParentFolderForeground { get; set; } = DarkParentFolderForegroundDefault;

    public bool ShouldSerializeParentFolderForeground()
    {
        return !EqualColor(this.ParentFolderForeground, DarkParentFolderForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeParentFolderForeground()
    {
        this.ParentFolderForeground = DarkParentFolderForegroundDefault;
    }

    #endregion

    #region Project Folders Background

    private static readonly Color LegacyProjectFoldersBackgroundDefault = Color.FromArgb(192, 218, 138);
    private static readonly Color DarkProjectFoldersBackgroundDefault = Color.FromArgb(92, 120, 69);

    [Category(ColorsCategoryName)]
    [DisplayName("Project folder background color")]
    [Description("Background color of Project folder element.")]
    public Color ProjectFoldersBackground { get; set; } = DarkProjectFoldersBackgroundDefault;

    public bool ShouldSerializeProjectFoldersBackground()
    {
        return !EqualColor(this.ProjectFoldersBackground, DarkProjectFoldersBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectFoldersBackground()
    {
        this.ProjectFoldersBackground = DarkProjectFoldersBackgroundDefault;
    }

    #endregion

    #region Project Folders Foreground

    private static readonly Color LegacyProjectFoldersForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkProjectFoldersForegroundDefault = Color.WhiteSmoke;

    [Category(ColorsCategoryName)]
    [DisplayName("Project folder text color")]
    [Description("Foreground color of Project folder element.")]
    public Color ProjectFoldersForeground { get; set; } = DarkProjectFoldersForegroundDefault;

    public bool ShouldSerializeProjectFoldersForeground()
    {
        return !EqualColor(this.ProjectFoldersForeground, DarkProjectFoldersForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectFoldersForeground()
    {
        this.ProjectFoldersForeground = DarkProjectFoldersForegroundDefault;
    }

    #endregion

    #region File Background

    private static readonly Color LegacyFileBreadcrumbBackgroundDefault = Color.FromArgb(0, 255, 255, 255);
    private static readonly Color DarkFileBreadcrumbBackgroundDefault = Color.FromArgb(0, 255, 255, 255);

    [Category(ColorsCategoryName)]
    [DisplayName("File background color")]
    [Description("Background color of File element.")]
    public Color FileBreadcrumbBackground { get; set; } = DarkFileBreadcrumbBackgroundDefault;

    public bool ShouldSerializeFileBreadcrumbBackground()
    {
        return !EqualColor(this.FileBreadcrumbBackground, DarkFileBreadcrumbBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeFileBreadcrumbBackground()
    {
        this.FileBreadcrumbBackground = DarkFileBreadcrumbBackgroundDefault;
    }

    #endregion

    #region File Foreground

    private static readonly Color LegacyFileBreadcrumbForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkFileBreadcrumbForegroundDefault = Color.Gainsboro;

    [Category(ColorsCategoryName)]
    [DisplayName("File text color")]
    [Description("Foreground color of File element.")]
    public Color FileBreadcrumbForeground { get; set; } = DarkFileBreadcrumbForegroundDefault;

    public bool ShouldSerializeFileBreadcrumbForeground()
    {
        return !EqualColor(this.FileBreadcrumbForeground, DarkFileBreadcrumbForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeFileBreadcrumbForeground()
    {
        this.FileBreadcrumbForeground = DarkFileBreadcrumbForegroundDefault;
    }

    #endregion

    #region Structure Background

    private static readonly Color LegacyStructureBreadcrumbBackgroundDefault = Color.FromArgb(0, 255, 255, 255);
    private static readonly Color DarkStructureBreadcrumbBackgroundDefault = Color.FromArgb(0, 255, 255, 255);

    [Category(ColorsCategoryName)]
    [DisplayName("Code structure element background color")]
    [Description("Background color of Code structureelement.")]
    public Color StructureBreadcrumbBackground { get; set; } = DarkStructureBreadcrumbBackgroundDefault;

    public bool ShouldSerializeStructureBreadcrumbBackground()
    {
        return !EqualColor(this.StructureBreadcrumbBackground, DarkStructureBreadcrumbBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeStructureBreadcrumbBackground()
    {
        this.StructureBreadcrumbBackground = DarkStructureBreadcrumbBackgroundDefault;
    }

    #endregion

    #region Structure Foreground

    private static readonly Color LegacyStructureBreadcrumbForegroundDefault = SystemColors.ControlText;
    private static readonly Color DarkStructureBreadcrumbForegroundDefault = Color.Gainsboro;

    [Category(ColorsCategoryName)]
    [DisplayName("Code structure element text color")]
    [Description("Foreground color of Code structure element.")]
    public Color StructureBreadcrumbForeground { get; set; } = DarkStructureBreadcrumbForegroundDefault;

    public bool ShouldSerializeStructureBreadcrumbForeground()
    {
        return !EqualColor(this.StructureBreadcrumbForeground, DarkStructureBreadcrumbForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeStructureBreadcrumbForeground()
    {
        this.StructureBreadcrumbForeground = DarkStructureBreadcrumbForegroundDefault;
    }

    #endregion

    // -------------------------------------------
    // Light appearance colors
    // -------------------------------------------
    [Category(ColorsCategoryName)]
    [DisplayName("Light theme solution root background color")]
    [Description("Background color of solution element in light editor appearance.")]
    public Color LightSolutionBackground { get; set; } = LegacySolutionBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme solution root text color")]
    [Description("Foreground color of solution element in light editor appearance.")]
    public Color LightSolutionForeground { get; set; } = LegacySolutionForegroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme non-solution root background color")]
    [Description("Background color of non-solution root element in light editor appearance.")]
    public Color LightNonSolutionRootBackground { get; set; } = LegacyNonSolutionRootBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme non-solution root text color")]
    [Description("Foreground color of non-solution root element in light editor appearance.")]
    public Color LightNonSolutionRootForeground { get; set; } = LegacyNonSolutionRootForegroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme project background color")]
    [Description("Background color of project name element in light editor appearance.")]
    public Color LightProjectBackground { get; set; } = LegacyProjectBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme project text color")]
    [Description("Foreground color of project name element in light editor appearance.")]
    public Color LightProjectForeground { get; set; } = LegacyProjectForegroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme solution folder background color")]
    [Description("Background color of solution folder element in light editor appearance.")]
    public Color LightSolutionFolderBackground { get; set; } = LegacySolutionFolderBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme solution folder text color")]
    [Description("Foreground color of solution folder element in light editor appearance.")]
    public Color LightSolutionFolderForeground { get; set; } = LegacySolutionFolderForegroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme parent folder background color")]
    [Description("Background color of parent folder element in light editor appearance.")]
    public Color LightParentFolderBackground { get; set; } = LegacyParentFolderBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme parent folder text color")]
    [Description("Foreground color of parent folder element in light editor appearance.")]
    public Color LightParentFolderForeground { get; set; } = LegacyParentFolderForegroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme project folder background color")]
    [Description("Background color of project folder element in light editor appearance.")]
    public Color LightProjectFoldersBackground { get; set; } = LegacyProjectFoldersBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme project folder text color")]
    [Description("Foreground color of project folder element in light editor appearance.")]
    public Color LightProjectFoldersForeground { get; set; } = LegacyProjectFoldersForegroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme file background color")]
    [Description("Background color of file element in light editor appearance.")]
    public Color LightFileBreadcrumbBackground { get; set; } = LegacyFileBreadcrumbBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme file text color")]
    [Description("Foreground color of file element in light editor appearance.")]
    public Color LightFileBreadcrumbForeground { get; set; } = LegacyFileBreadcrumbForegroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme code structure element background color")]
    [Description("Background color of code structure element in light editor appearance.")]
    public Color LightStructureBreadcrumbBackground { get; set; } = LegacyStructureBreadcrumbBackgroundDefault;

    [Category(ColorsCategoryName)]
    [DisplayName("Light theme code structure element text color")]
    [Description("Foreground color of code structure element in light editor appearance.")]
    public Color LightStructureBreadcrumbForeground { get; set; } = LegacyStructureBreadcrumbForegroundDefault;

    public EditorBarColorSet GetColorSet(EditorColorMode mode)
    {
        return mode switch
        {
            EditorColorMode.Light => new EditorBarColorSet
            {
                SolutionBackground = this.LightSolutionBackground,
                SolutionForeground = this.LightSolutionForeground,
                NonSolutionRootBackground = this.LightNonSolutionRootBackground,
                NonSolutionRootForeground = this.LightNonSolutionRootForeground,
                ProjectBackground = this.LightProjectBackground,
                ProjectForeground = this.LightProjectForeground,
                SolutionFolderBackground = this.LightSolutionFolderBackground,
                SolutionFolderForeground = this.LightSolutionFolderForeground,
                ParentFolderBackground = this.LightParentFolderBackground,
                ParentFolderForeground = this.LightParentFolderForeground,
                ProjectFoldersBackground = this.LightProjectFoldersBackground,
                ProjectFoldersForeground = this.LightProjectFoldersForeground,
                FileBreadcrumbBackground = this.LightFileBreadcrumbBackground,
                FileBreadcrumbForeground = this.LightFileBreadcrumbForeground,
                StructureBreadcrumbBackground = this.LightStructureBreadcrumbBackground,
                StructureBreadcrumbForeground = this.LightStructureBreadcrumbForeground
            },
            EditorColorMode.Dark => new EditorBarColorSet
            {
                SolutionBackground = this.SolutionBackground,
                SolutionForeground = this.SolutionForeground,
                NonSolutionRootBackground = this.NonSolutionRootBackground,
                NonSolutionRootForeground = this.NonSolutionRootForeground,
                ProjectBackground = this.ProjectBackground,
                ProjectForeground = this.ProjectForeground,
                SolutionFolderBackground = this.SolutionFolderBackground,
                SolutionFolderForeground = this.SolutionFolderForeground,
                ParentFolderBackground = this.ParentFolderBackground,
                ParentFolderForeground = this.ParentFolderForeground,
                ProjectFoldersBackground = this.ProjectFoldersBackground,
                ProjectFoldersForeground = this.ProjectFoldersForeground,
                FileBreadcrumbBackground = this.FileBreadcrumbBackground,
                FileBreadcrumbForeground = this.FileBreadcrumbForeground,
                StructureBreadcrumbBackground = this.StructureBreadcrumbBackground,
                StructureBreadcrumbForeground = this.StructureBreadcrumbForeground
            },
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private bool HasLegacySharedDarkPalette()
    {
        return EqualColor(this.SolutionBackground, LegacySolutionBackgroundDefault)
            && EqualColor(this.SolutionForeground, LegacySolutionForegroundDefault)
            && EqualColor(this.NonSolutionRootBackground, LegacyNonSolutionRootBackgroundDefault)
            && EqualColor(this.NonSolutionRootForeground, LegacyNonSolutionRootForegroundDefault)
            && EqualColor(this.ProjectBackground, LegacyProjectBackgroundDefault)
            && EqualColor(this.ProjectForeground, LegacyProjectForegroundDefault)
            && EqualColor(this.SolutionFolderBackground, LegacySolutionFolderBackgroundDefault)
            && EqualColor(this.SolutionFolderForeground, LegacySolutionFolderForegroundDefault)
            && EqualColor(this.ParentFolderBackground, LegacyParentFolderBackgroundDefault)
            && EqualColor(this.ParentFolderForeground, LegacyParentFolderForegroundDefault)
            && EqualColor(this.ProjectFoldersBackground, LegacyProjectFoldersBackgroundDefault)
            && EqualColor(this.ProjectFoldersForeground, LegacyProjectFoldersForegroundDefault)
            && EqualColor(this.FileBreadcrumbBackground, LegacyFileBreadcrumbBackgroundDefault)
            && EqualColor(this.FileBreadcrumbForeground, LegacyFileBreadcrumbForegroundDefault)
            && EqualColor(this.StructureBreadcrumbBackground, LegacyStructureBreadcrumbBackgroundDefault)
            && EqualColor(this.StructureBreadcrumbForeground, LegacyStructureBreadcrumbForegroundDefault);
    }

    private void ApplyDarkThemeDefaults()
    {
        this.SolutionBackground = DarkSolutionBackgroundDefault;
        this.SolutionForeground = DarkSolutionForegroundDefault;
        this.NonSolutionRootBackground = DarkNonSolutionRootBackgroundDefault;
        this.NonSolutionRootForeground = DarkNonSolutionRootForegroundDefault;
        this.ProjectBackground = DarkProjectBackgroundDefault;
        this.ProjectForeground = DarkProjectForegroundDefault;
        this.SolutionFolderBackground = DarkSolutionFolderBackgroundDefault;
        this.SolutionFolderForeground = DarkSolutionFolderForegroundDefault;
        this.ParentFolderBackground = DarkParentFolderBackgroundDefault;
        this.ParentFolderForeground = DarkParentFolderForegroundDefault;
        this.ProjectFoldersBackground = DarkProjectFoldersBackgroundDefault;
        this.ProjectFoldersForeground = DarkProjectFoldersForegroundDefault;
        this.FileBreadcrumbBackground = DarkFileBreadcrumbBackgroundDefault;
        this.FileBreadcrumbForeground = DarkFileBreadcrumbForegroundDefault;
        this.StructureBreadcrumbBackground = DarkStructureBreadcrumbBackgroundDefault;
        this.StructureBreadcrumbForeground = DarkStructureBreadcrumbForegroundDefault;
    }
}
