using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Options;

internal partial class OptionsProvider
{
    // Register the options with this attribute on your package class:
    // [ProvideOptionPage(typeof(OptionsProvider.GeneralPageOptions), "EditorBar.Options", "GeneralPage", 0, 0, true, SupportsProfiles = true)]
    [ComVisible(true)]
    public class GeneralPageOptions : BaseOptionPage<GeneralPage> { }
}

public class GeneralPage : BaseOptionModel<GeneralPage>
{
    [Category("Apperance")]
    [DisplayName("Bar position")]
    [Description("Position of the editor bar (change will be applied to a newly opened documents)")]
    [DefaultValue(Options.BarPosition.Top)]
    [TypeConverter(typeof(EnumConverter))]
    public BarPosition BarPosition { get; set; } = BarPosition.Top;
        
    [Category("Apperance")]
    [DisplayName("Show relative path")]
    [Description("Show path relative to the solution root")]
    [DefaultValue(true)]
    public bool ShowPathRelativeToSolutionRoot { get; set; } = true;

    public override Task SaveAsync()
    {
        Console.WriteLine("Saving...");
        return base.SaveAsync();
    }
}

public enum BarPosition
{
    Top = 0,
    Bottom = 1
}