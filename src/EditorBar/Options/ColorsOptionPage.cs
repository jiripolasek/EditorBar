// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using JPSoftworks.EditorBar.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Represents the colors options page.
/// </summary>
[ComVisible(true)]
[Guid("7e8f3cb1-7771-4d3c-8c5f-a81cc2b3e9df")]
public class ColorsOptionPage : UIElementDialogPage
{
    private static readonly MethodInfo DialogPageOnPropertyChangedMethod =
        typeof(DialogPage).GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private readonly GeneralOptionPageSettings _settings = new();

    private ColorsOptionsControl? _control;

    /// <summary>
    /// Gets the automation object whose properties Visual Studio persists for roaming and Import/Export.
    /// </summary>
    [Browsable(false)]
    public override object AutomationObject => this._settings;

    /// <summary>
    /// Gets the child element of the options page.
    /// </summary>
    protected override UIElement Child
    {
        get
        {
            if (this._control == null)
            {
                this._control = new ColorsOptionsControl();
                this._control.SettingsChanged += this.OnControlSettingsChanged;
                this._control.Initialize(this._settings);
            }

            return this._control;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorsOptionPage" /> class.
    /// </summary>
    public ColorsOptionPage()
    {
        GeneralOptionsModel.Saved += _ =>
        {
            this._settings.CopyFromModel(GeneralOptionsModel.Instance);
            this._control?.Initialize(this._settings);
        };
    }

    /// <summary>
    /// Handles page activation.
    /// </summary>
    /// <param name="e">The activation event data.</param>
    protected override void OnActivate(CancelEventArgs e)
    {
        base.OnActivate(e);
        this._control?.Initialize(this._settings);
    }

    /// <summary>
    /// Loads the settings from storage.
    /// </summary>
    public override void LoadSettingsFromStorage()
    {
        this._settings.CopyFromModel(GeneralOptionsModel.Instance);
        base.LoadSettingsFromStorage();
        this._settings.ApplyToModel(GeneralOptionsModel.Instance);
        this._control?.Initialize(this._settings);
    }

    /// <summary>
    /// Loads the settings from a Visual Studio settings file.
    /// </summary>
    /// <param name="reader">The Visual Studio settings reader.</param>
    public override void LoadSettingsFromXml(IVsSettingsReader reader)
    {
        base.LoadSettingsFromXml(reader);
        this._settings.ApplyToModel(GeneralOptionsModel.Instance);
        GeneralOptionsModel.Instance.Save();
        this._control?.Initialize(this._settings);
    }

    /// <summary>
    /// Saves the settings to storage.
    /// </summary>
    public override void SaveSettingsToStorage()
    {
        this._control?.CopyToSettings(this._settings);
        this._settings.ApplyToModel(GeneralOptionsModel.Instance);
        GeneralOptionsModel.Instance.Save();
        base.SaveSettingsToStorage();
    }

    /// <summary>
    /// Applies the settings from the custom colors UI even when the page dirty state is not inferred automatically.
    /// </summary>
    /// <param name="e">The apply event data.</param>
    protected override void OnApply(PageApplyEventArgs e)
    {
        this._control?.CopyToSettings(this._settings);
        this._settings.ApplyToModel(GeneralOptionsModel.Instance);
        GeneralOptionsModel.Instance.Save();
        base.OnApply(e);
    }

    private void OnControlSettingsChanged(object? sender, EventArgs e)
    {
        this._control?.CopyToSettings(this._settings);
        DialogPageOnPropertyChangedMethod.Invoke(this, [this._settings, EventArgs.Empty]);
    }
}
