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
    private readonly GeneralOptionPageSettings _settings = new();

    private GeneralOptionsControl? _control;
    private bool _isSavingSettingsToStorage;

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
        GeneralOptionsModel.Saved += this.GeneralOptionsModelOnSaved;
    }

    /// <summary>
    /// Handles page activation.
    /// </summary>
    /// <param name="e">The activation event data.</param>
    protected override void OnActivate(CancelEventArgs e)
    {
        base.OnActivate(e);
        this._control?.Initialize();
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
        try
        {
            this._isSavingSettingsToStorage = true;
            this._control?.Apply();
            this._settings.CopyFromModel(GeneralOptionsModel.Instance);
            base.SaveSettingsToStorage();
        }
        finally
        {
            this._isSavingSettingsToStorage = false;
        }
    }

    private void GeneralOptionsModelOnSaved(GeneralOptionsModel model)
    {
        ThreadHelper.JoinableTaskFactory.Run(
            async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                this._settings.CopyFromModel(model);
                this.SaveAutomationSettingsToStorage();
                this._control?.Initialize();
            });
    }

    private void SaveAutomationSettingsToStorage()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (this._isSavingSettingsToStorage)
        {
            return;
        }

        try
        {
            this._isSavingSettingsToStorage = true;
            base.SaveSettingsToStorage();
        }
        finally
        {
            this._isSavingSettingsToStorage = false;
        }
    }
}
