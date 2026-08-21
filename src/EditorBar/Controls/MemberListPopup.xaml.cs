// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for MemberListPopup.xaml
/// </summary>
public partial class MemberListPopup : Popup
{
    private static readonly AsyncLocal<MemberListPopup?> CurrentMenuInteractionPopupHolder = new();
    private int _suspendAutoCloseCount;

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content), typeof(UIElement), typeof(MemberListPopup), new PropertyMetadata(default(UIElement)));

    internal static MemberListPopup? CurrentMenuInteractionPopup => CurrentMenuInteractionPopupHolder.Value;

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
        this._suspendAutoCloseCount = 0;
        this.StaysOpen = false;
        if (Equals(this, Mouse.Captured!))
        {
            Mouse.Capture(null!);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.IsOpen = false;

            var symbolChevronButton = this.FindAncestor<SymbolChevronButton>();
            if (symbolChevronButton != null)
            {
                FocusOwnerAsync(symbolChevronButton).FireAndForget();
            }

            e.Handled = true;
        }

        base.OnKeyDown(e);
    }

    private static async Task FocusOwnerAsync(SymbolChevronButton symbolChevronButton)
    {
        await Task.Yield();
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        //symbolChevronButton.AcquireWin32Focus(out _);
        symbolChevronButton.FocusButton();
    }

    internal IDisposable SuspendAutoClose()
    {
        this._suspendAutoCloseCount++;
        this.StaysOpen = true;
        return new SuspendAutoCloseScope(this);
    }

    internal static IDisposable EnterMenuInteraction(MemberListPopup popup)
    {
        var previousPopup = CurrentMenuInteractionPopupHolder.Value;
        CurrentMenuInteractionPopupHolder.Value = popup;
        return new MenuInteractionScope(previousPopup);
    }

    private void ResumeAutoClose()
    {
        if (this._suspendAutoCloseCount > 0)
        {
            this._suspendAutoCloseCount--;
        }

        if (this._suspendAutoCloseCount == 0)
        {
            this.StaysOpen = false;
        }
    }

    private sealed class SuspendAutoCloseScope(MemberListPopup popup) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            popup.ResumeAutoClose();
            this._disposed = true;
        }
    }

    private sealed class MenuInteractionScope(MemberListPopup? previousPopup) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            CurrentMenuInteractionPopupHolder.Value = previousPopup;
            this._disposed = true;
        }
    }
}
