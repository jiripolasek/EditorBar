// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

#pragma warning disable IDE0079 // Remove unnecessary suppression (IDE0079)

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Model specifies general options for the Editor Bar.
/// </summary>
/// <seealso cref="BaseOptionModel{GeneralPage}" />
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used implicitly.")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Setters are used implicitly by PropertyGrid.")]
public class GeneralOptionsModel : BaseOptionModel<GeneralOptionsModel>, IRatingConfig
{
    private const int CurrentConfigVersion = 2;

    private const string AppearanceCategoryName = "Appearance";
    private const string GeneralCategoryName = "General";
    private const string ColorsCategoryName = "Colors";
    private const string AdditionalActionCategoryName = "Actions";
    private const string ExternalEditorCategoryName = "External Editor";

    // keep legacy name to  keep settings for existing users intact
    protected override string CollectionName => "JPSoftworks.EditorBar.Options.GeneralPage";

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
    public FileLabel FileLabelStyle { get; set; } = FileLabel.FileName;

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
    [Description("Choose the style of the Editor Bar. Normal mode is more specious, compact mode gives you more vertical space for you code.")]
    [DefaultValue(DisplayStyle.Normal)]
    public DisplayStyle DisplayStyle { get; set; } = DisplayStyle.Normal;

    [Category(AppearanceCategoryName)]
    [DisplayName("Visual Style")]
    [Description("Choose the theme of the Editor Bar.")]
    [DefaultValue(VisualStyle.FullRowTransparent)]
    public VisualStyle VisualStyle { get; set; } = VisualStyle.FullRowTransparent;

    // -------------------------------------------
    // General category
    // -------------------------------------------

    [Category(GeneralCategoryName)]
    [DisplayName("Enable Editor Bar")]
    [Description("Determines if the Editor Bar is visible.")]
    [DefaultValue(true)]
    public bool Enabled { get; set; } = true;


    private static bool EqualColor(Color left, Color right)
    {
        return left.ToArgb() == right.ToArgb();
    }

    // -------------------------------------------
    // Colors category
    // -------------------------------------------

    // Comparison of System.Drawing.Colors is little bit funny: Color.Black != Color.FromArgb(0, 0, 0)
    // So let's do comparison by ARGB values. See ColorEquality method below.

    #region Solution Background

    private static readonly Color SolutionBackgroundDefault = Color.Purple;

    [Category(ColorsCategoryName)]
    [DisplayName("Solution root background color")]
    [Description("Background color of solution element.")]
    public Color SolutionBackground { get; set; } = SolutionBackgroundDefault;

    public bool ShouldSerializeSolutionBackground()
    {
        return !EqualColor(this.SolutionBackground, SolutionBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionBackground()
    {
        this.SolutionBackground = SolutionBackgroundDefault;
    }

    #endregion


    #region Solution Foreground

    private static readonly Color SolutionForegroundDefault = SystemColors.ControlText;

    [Category(ColorsCategoryName)]
    [DisplayName("Solution root text Color")]
    [Description("Foreground color of solution element.")]
    public Color SolutionForeground { get; set; } = SolutionForegroundDefault;

    public bool ShouldSerializeSolutionForeground()
    {
        return !EqualColor(this.SolutionForeground, SolutionForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionForeground()
    {
        this.SolutionForeground = SolutionForegroundDefault;
    }

    #endregion


    #region NonSolutionRoot Background

    private static readonly Color NonSolutionRootBackgroundDefault = Color.Silver;

    [Category(ColorsCategoryName)]
    [DisplayName("NonSolutionRoot root background color")]
    [Description("Background color of NonSolutionRoot element.")]
    public Color NonSolutionRootBackground { get; set; } = NonSolutionRootBackgroundDefault;

    public bool ShouldSerializeNonSolutionRootBackground()
    {
        return !EqualColor(this.NonSolutionRootBackground, NonSolutionRootBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeNonSolutionRootBackground()
    {
        this.NonSolutionRootBackground = NonSolutionRootBackgroundDefault;
    }

    #endregion


    #region NonSolutionRoot Foreground

    private static readonly Color NonSolutionRootForegroundDefault = SystemColors.ControlText;

    [Category(ColorsCategoryName)]
    [DisplayName("NonSolutionRoot root text Color")]
    [Description("Foreground color of NonSolutionRoot element.")]
    public Color NonSolutionRootForeground { get; set; } = NonSolutionRootForegroundDefault;

    public bool ShouldSerializeNonSolutionRootForeground()
    {
        return !EqualColor(this.NonSolutionRootForeground, NonSolutionRootForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeNonSolutionRootForeground()
    {
        this.NonSolutionRootForeground = NonSolutionRootForegroundDefault;
    }

    #endregion


    #region Project Background

    private static readonly Color ProjectBackgroundDefault = Color.LightSkyBlue;

    [Category(ColorsCategoryName)]
    [DisplayName("Project background color")]
    [Description("Background color of project name element.")]
    public Color ProjectBackground { get; set; } = ProjectBackgroundDefault;

    public bool ShouldSerializeProjectBackground()
    {
        return !EqualColor(this.ProjectBackground, ProjectBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectBackground()
    {
        this.ProjectBackground = ProjectBackgroundDefault;
    }

    #endregion


    #region Project Foreground

    private static readonly Color ProjectForegroundDefault = SystemColors.ControlText;

    [Category(ColorsCategoryName)]
    [DisplayName("Project text color")]
    [Description("Foreground color of project name.")]
    public Color ProjectForeground { get; set; } = ProjectForegroundDefault;

    public bool ShouldSerializeProjectForeground()
    {
        return !EqualColor(this.ProjectForeground, ProjectForegroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectForeground()
    {
        this.ProjectForeground = ProjectForegroundDefault;
    }

    #endregion


    #region Solution Folder Background

    private static readonly Color SolutionFolderBackgroundDefault = Color.Gold;

    [Category(ColorsCategoryName)]
    [DisplayName("Solution folder background color")]
    [Description("Background color of solution folder element.")]
    public Color SolutionFolderBackground { get; set; } = SolutionFolderBackgroundDefault;

    public bool ShouldSerializeSolutionFolderBackground()
    {
        return !EqualColor(this.SolutionFolderBackground, SolutionFolderBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionFolderBackground()
    {
        this.SolutionFolderBackground = SolutionFolderBackgroundDefault;
    }

    #endregion


    #region Solution Folder Foreground

    private static readonly Color SolutionFolderForegroundDefaultColor = SystemColors.ControlText;

    [Category(ColorsCategoryName)]
    [DisplayName("Solution folder text color")]
    [Description("Foreground color of solution folder element.")]
    public Color SolutionFolderForeground { get; set; } = SolutionFolderForegroundDefaultColor;

    public bool ShouldSerializeSolutionFolderForeground()
    {
        return !EqualColor(this.SolutionFolderForeground, SolutionFolderForegroundDefaultColor);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeSolutionFolderForeground()
    {
        this.SolutionFolderForeground = SolutionFolderForegroundDefaultColor;
    }

    #endregion


    #region Parent Folder Background

    private static readonly Color ParentFolderBackgroundDefault = Color.YellowGreen;

    [Category(ColorsCategoryName)]
    [DisplayName("Parent folder background color")]
    [Description("Background color of Parent folder element.")]
    public Color ParentFolderBackground { get; set; } = ParentFolderBackgroundDefault;

    public bool ShouldSerializeParentFolderBackground()
    {
        return !EqualColor(this.ParentFolderBackground, ParentFolderBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeParentFolderBackground()
    {
        this.ParentFolderBackground = ParentFolderBackgroundDefault;
    }

    #endregion


    #region Parent Folder Foreground

    private static readonly Color ParentFolderForegroundDefaultColor = SystemColors.ControlText;

    [Category(ColorsCategoryName)]
    [DisplayName("Parent folder text color")]
    [Description("Foreground color of Parent folder element.")]
    public Color ParentFolderForeground { get; set; } = ParentFolderForegroundDefaultColor;

    public bool ShouldSerializeParentFolderForeground()
    {
        return !EqualColor(this.ParentFolderForeground, ParentFolderForegroundDefaultColor);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeParentFolderForeground()
    {
        this.ParentFolderForeground = ParentFolderForegroundDefaultColor;
    }

    #endregion


    #region Project Folders Background

    private static readonly Color ProjectFoldersBackgroundDefault = Color.FromArgb(192, 218, 138);

    [Category(ColorsCategoryName)]
    [DisplayName("Project folder background color")]
    [Description("Background color of Project folder element.")]
    public Color ProjectFoldersBackground { get; set; } = ProjectFoldersBackgroundDefault;

    public bool ShouldSerializeProjectFoldersBackground()
    {
        return !EqualColor(this.ProjectFoldersBackground, ProjectFoldersBackgroundDefault);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectFoldersBackground()
    {
        this.ProjectFoldersBackground = ProjectFoldersBackgroundDefault;
    }

    #endregion

    #region Project Folders Foreground

    private static readonly Color ProjectFoldersForegroundDefaultColor = SystemColors.ControlText;

    [Category(ColorsCategoryName)]
    [DisplayName("Project folder text color")]
    [Description("Foreground color of Project folder element.")]
    public Color ProjectFoldersForeground { get; set; } = ProjectFoldersForegroundDefaultColor;

    public bool ShouldSerializeProjectFoldersForeground()
    {
        return !EqualColor(this.ProjectFoldersForeground, ProjectFoldersForegroundDefaultColor);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void ResetSerializeProjectFoldersForeground()
    {
        this.ProjectFoldersForeground = ProjectFoldersForegroundDefaultColor;
    }

    #endregion

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
    [Description("Path to external editor or command. " + Launcher.FileNamePlaceholderConstant + " represents the file name.")]
    [DefaultValue(Launcher.FileNamePlaceholderConstant)]
    public string? ExternalEditorCommandArguments { get; set; } = Launcher.FileNamePlaceholderConstant;

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
            // Sequential upgrade accross config versions
            if (this.Version < 2)
            {
                // When upgrading from version 1 to 2, we need to change the default value of FileLabelStyle
                // If the user had relative paths enabled, then we set the FileLabelStyle to FileName (not relative), because with this update
                // we are also adding breadcrumbs for in-project folders and parent folder that supersedes the need for relative paths.
                // User can disable these new features and revert to relative paths if they want manually.
                //
                // For absolute path, let's just keep the setting as it is. User might be anoyed by the long paths, which may force them to
                // go to settings. This should be "fixed" later by adding What's new dialog.
                this.FileLabelStyle = this.ShowPathRelativeToSolutionRoot ? FileLabel.FileName : FileLabel.AbsolutePath;
            }

            this.Version = CurrentConfigVersion;
            changed = true;
        }

        if (changed)
        {
            await this.SaveAsync();
        }
    }
}