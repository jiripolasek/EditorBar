// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using JPSoftworks.EditorBar.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Represents the general options page.
/// </summary>
[ComVisible(true)]
[Guid("5ccfe329-366c-4759-9bf2-cf97fec6d28c")]
public class GeneralOptionPage : UIElementDialogPage
{
    private GeneralOptionsControl? _control;
    private readonly GeneralOptionPageSettings _settings = new();

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
                this._control = new GeneralOptionsControl();
                this._control.Initialize();
            }

            return this._control;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralOptionPage" /> class.
    /// </summary>
    public GeneralOptionPage()
    {
        // Subscribe to the Saved event to reinitialize the control when the settings are saved.
        // LoadSettingsFromStorage method is invoked only once when the page is created - once in VS lifetime,
        // then it's just reused. (tl;dr: it is NOT invoked when Options dialog is opened). OnActivate event is
        // on the other hand invoked every time the page is accessed, even when switching between different pages.
        // So we need to reinitialize the control when the settings are saved.
        GeneralOptionsModel.Saved += _ =>
        {
            this._control?.Initialize();
        };
    }

    /// <summary>
    /// Loads the settings from storage.
    /// </summary>
    public override void LoadSettingsFromStorage()
    {
        this._settings.CopyFromModel(GeneralOptionsModel.Instance);
        base.LoadSettingsFromStorage();
        this._settings.ApplyToModel(GeneralOptionsModel.Instance);
        this._control?.Initialize();
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
        this._control?.Initialize();
    }

    /// <summary>
    /// Saves the settings to storage.
    /// </summary>
    public override void SaveSettingsToStorage()
    {
        this._control?.Apply();
        this._settings.CopyFromModel(GeneralOptionsModel.Instance);
        base.SaveSettingsToStorage();
    }
}
