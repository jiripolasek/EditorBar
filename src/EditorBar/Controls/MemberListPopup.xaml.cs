// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualStudio.PlatformUI;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for MemberListPopup.xaml
/// </summary>
public partial class MemberListPopup : Popup
{
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content), typeof(UIElement), typeof(MemberListPopup), new PropertyMetadata(default(UIElement)));

    public UIElement Content
    {
        get => (UIElement)this.GetValue(ContentProperty);
        set => this.SetValue(ContentProperty, value);
    }

    public MemberListPopup()
    {
        this.InitializeComponent();

        KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Local);
        KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.None);
        KeyboardNavigation.SetControlTabNavigation(this, KeyboardNavigationMode.None);
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        this.Child?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        if (Equals(this, Mouse.Captured!))
        {
            Mouse.Capture(null!);
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.IsOpen = false;

            var symbolChevronButton = this.FindAncestor<SymbolChevronButton>();
            if (symbolChevronButton != null)
            {
                _ = this.Dispatcher!.BeginInvoke(() =>
                {
                    //symbolChevronButton.AcquireWin32Focus(out _);
                    symbolChevronButton.FocusButton();
                }, DispatcherPriority.ApplicationIdle);
            }

            e.Handled = true;
        }

        base.OnPreviewKeyDown(e);
    }
}