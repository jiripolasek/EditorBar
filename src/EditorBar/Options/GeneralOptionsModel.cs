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

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Model specifies general options for the Editor Bar.
/// </summary>
/// <seealso cref="BaseOptionModel{GeneralPage}" />
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used implicitly.")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Setters are used implicitly by PropertyGrid.")]
public class GeneralOptionsModel : BaseOptionModel<GeneralOptionsModel>
{
    // keep legacy name to  keep settings for existing users intact
    protected override string CollectionName => "JPSoftworks.EditorBar.Options.GeneralPage";

    private const string AppearanceCategoryName = "Appearance";
    private const string GeneralCategoryName = "General";
    private const string ColorsCategoryName = "Colors";
    private const string AdditionalActionCategoryName = "Actions";
    private const string ExternalEditorCategoryName = "External Editor";

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
    [DisplayName("Show solution folders")]
    [Description("Show solution folder block elements in the Editor Bar.")]
    [DefaultValue(true)]
    public bool ShowSolutionFolders { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("Show solution root")]
    [Description("Show solution root element in the Editor Bar.")]
    [DefaultValue(true)]
    public bool ShowSolutionRoot { get; set; } = true;

    [Category(AppearanceCategoryName)]
    [DisplayName("Display mode")]
    [Description("Choose the style of the Editor Bar. Normal mode is more specious, compact mode gives you more vertical space for you code.")]
    [DefaultValue(DisplayStyle.Normal)]
    public DisplayStyle DisplayStyle { get; set; } = DisplayStyle.Normal;

    [Category(AppearanceCategoryName)]
    [DisplayName("Visual Style")]
    [Description("Choose the theme of the Editor Bar.")]
    [DefaultValue(VisualStyle.FullRowCommandBar)]
    public VisualStyle VisualStyle { get; set; } = VisualStyle.FullRowCommandBar;

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
}