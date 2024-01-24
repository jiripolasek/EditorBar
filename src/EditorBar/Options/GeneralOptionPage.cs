// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Windows;
using JPSoftworks.EditorBar.Controls;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Represents the general options page.
/// </summary>
[ComVisible(true)]
[Guid("5ccfe329-366c-4759-9bf2-cf97fec6d28c")]
public class GeneralOptionPage : UIElementDialogPage
{
    private GeneralOptionsControl? _control;

    /// <summary>
    /// Gets or sets the child element of the options page.
    /// </summary>
    protected override UIElement Child => this._control ??= new GeneralOptionsControl();

    /// <summary>
    /// Loads the settings from storage.
    /// </summary>
    public override void LoadSettingsFromStorage()
    {
        base.LoadSettingsFromStorage();
        this._control?.Initialize();
    }

    /// <summary>
    /// Saves the settings to storage.
    /// </summary>
    public override void SaveSettingsToStorage()
    {
        base.SaveSettingsToStorage();
        this._control?.Apply();
    }
}
