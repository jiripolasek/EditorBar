// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Represents the general options control for the EditorBar extension.
/// </summary>
public partial class GeneralOptionsControl
{
    /// <summary>
    /// Gets the view model for the options page.
    /// </summary>
    public OptionsPageViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralOptionsControl" /> class.
    /// </summary>
    public GeneralOptionsControl()
    {
        this.ViewModel = new OptionsPageViewModel();
        this.DataContext = this;
        this.InitializeComponent();
    }

    /// <summary>
    /// Initializes the control by loading the settings from the model.
    /// </summary>
    public void Initialize()
    {
        this.ViewModel.Load(GeneralOptionsModel.Instance);
    }

    /// <summary>
    /// Applies the current settings to the model.
    /// </summary>
    public void Apply()
    {
        this.ViewModel.Save(GeneralOptionsModel.Instance);
    }

    /// <summary>
    /// Handles the click event of the CustomizeKeyboard hyperlink.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void CustomizeKeyboardHyperlink_OnClick(object sender, RoutedEventArgs e)
    {
        VS.Commands.ExecuteAsync(VSConstants.VSStd97CmdID.CustomizeKeyboard).FireAndForget();
    }
}