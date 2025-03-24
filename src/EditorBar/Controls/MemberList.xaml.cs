// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows.Controls;
using System.Windows.Input;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for MemberList.xaml
/// </summary>
public partial class MemberList : UserControl
{
    public event EventHandler? ItemSelected;

    public MemberList()
    {
        this.InitializeComponent();
    }

    public MemberList(IEnumerable<MemberListItemViewModel> members)
    {
        this.InitializeComponent();
        this.ListBox!.ItemsSource = members;
    }

    private void ListBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Enter or Key.Space)
        {
            this.OnItemSelected();
        }
    }

    private void OnItemSelected()
    {
        this.ItemSelected?.Invoke(this, EventArgs.Empty!);
    }

    private void ListBox_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (this.ListBox!.SelectedItem != null)
        {
            this.OnItemSelected();
        }
    }
}