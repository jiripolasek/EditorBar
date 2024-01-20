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
public class GeneralPage : BaseOptionModel<GeneralPage>
{
    private const string AppearanceCategoryName = "Appearance";
    private const string GeneralCategoryName = "General";
    private const string ColorsCategoryName = "Colors";

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
}