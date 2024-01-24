// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.ViewModels;

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
}