// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Model specifies general options for the Editor Bar.
/// </summary>
/// <seealso cref="Community.VisualStudio.Toolkit.BaseOptionModel{GeneralPage}" />
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used implicitly.")]
public class GeneralPage : BaseOptionModel<GeneralPage>
{
    private const string AppearanceCategoryName = "Appearance";
    private const string GeneralCategoryName = "General";

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
    [DisplayName("Display mode")]
    [Description("Choose the style of the Editor Bar. Normal mode is more specious, compact mode gives you more vertical space for you code.")]
    [DefaultValue(DisplayStyle.Normal)]
    public DisplayStyle DisplayStyle { get; set; } = DisplayStyle.Normal;

    [Category(GeneralCategoryName)]
    [DisplayName("Enable Editor Bar")]
    [Description("Determines if the Editor Bar is visible.")]
    [DefaultValue(true)]
    public bool Enabled { get; set; } = true;
}