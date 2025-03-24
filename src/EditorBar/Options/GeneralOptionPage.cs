// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

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