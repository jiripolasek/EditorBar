// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.ComponentModel;
using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Options;

public class GeneralPage : BaseOptionModel<GeneralPage>
{
    [Category("Appearance")]
    [DisplayName("Bar position")]
    [Description("Position of the editor bar (change will be applied to a newly opened documents).")]
    [DefaultValue(Options.BarPosition.Top)]
    [TypeConverter(typeof(EnumConverter))]
    public BarPosition BarPosition { get; set; } = BarPosition.Top;

    [Category("Appearance")]
    [DisplayName("Show relative path")]
    [Description("Show path relative to the solution root.")]
    [DefaultValue(true)]
    public bool ShowPathRelativeToSolutionRoot { get; set; } = true;

    public override Task SaveAsync()
    {
        Console.WriteLine("Saving...");
        return base.SaveAsync();
    }
}