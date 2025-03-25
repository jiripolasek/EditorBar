// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows.Input;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Control for displaying a file label.
/// </summary>
internal partial class LegacyFileLabel
{
    public LegacyLabelViewModel? ViewModel => this.DataContext as LegacyLabelViewModel;

    public LegacyFileLabel()
    {
        this.InitializeComponent();
    }

    private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        this.ViewModel?.ContextMenuCommand.Execute(null!);
    }

    private void PathLabel_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        var command = isControlPressed ? this.ViewModel?.SecondaryCommand : this.ViewModel?.PrimaryCommand;
        command?.Execute(null!);
    }
}