// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Windows;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Controls;
/// <summary>
/// Interaction logic for GeneralOptions.xaml
/// </summary>
public partial class GeneralOptionsControl
{
    public OptionsPageViewModel ViewModel { get; }

    public GeneralOptionsControl()
    {
        this.ViewModel = new OptionsPageViewModel();
        this.DataContext = this;
        this.InitializeComponent();
    }

    public void Initialize()
    {
        this.ViewModel.Load(GeneralOptionsModel.Instance);
    }

    public void Apply()
    {
        this.ViewModel.Save(GeneralOptionsModel.Instance);
    }

    private void CustomizeKeyboardHyperlink_OnClick(object sender, RoutedEventArgs e)
    {
        VS.Commands.ExecuteAsync(VSConstants.VSStd97CmdID.CustomizeKeyboard).FireAndForget();
    }
}