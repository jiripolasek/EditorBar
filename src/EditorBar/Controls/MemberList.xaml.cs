// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for MemberList.xaml
/// </summary>
public partial class MemberList : UserControl
{
    public event EventHandler? ItemSelected;

    private readonly CollectionViewSource _collectionViewSource;

    public MemberList()
    {
        this.InitializeComponent();
        this._collectionViewSource = new CollectionViewSource();
        this.ListBox!.ItemsSource = this._collectionViewSource.View;
        this.UpdateFilterPredicate();
    }

    public MemberList(IEnumerable<MemberListItemViewModel> members) : this()
    {
        this._collectionViewSource.Source = members;
        this.ListBox!.ItemsSource = this._collectionViewSource.View;
        this.UpdateFilterPredicate();
    }

    private void ListBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape)
        {
            this.HandleEscape(e);
        }
        else if (e.Key is Key.Enter or Key.Space)
        {
            this.OnItemSelected();
            e.Handled = true;
        }
        else if (IsCtrlEdgeNavigation(e))
        {
            this.MoveSelectionToEdge(e.Key == Key.Up);
            e.Handled = true;
        }
        else if (IsTextInputKey(e))
        {
            this.ShowFilterAndForwardKey(e);
        }
    }

    private static bool IsCtrlEdgeNavigation(KeyEventArgs e)
    {
        return (e.Key == Key.Up || e.Key == Key.Down) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
    }

    private static bool IsTextInputKey(KeyEventArgs e)
    {
        // Ignore modifier-only keys and navigation
        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
            e.Key == Key.LeftShift || e.Key == Key.RightShift ||
            e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
            e.Key == Key.Tab || e.Key == Key.Escape ||
            e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
        {
            return false;
        }

        // Let PreviewTextInput handle most text, but include Back/Del
        return e.Key == Key.Back || e.Key == Key.Delete || Keyboard.Modifiers == ModifierKeys.None;
    }

    private void ShowFilterAndForwardKey(KeyEventArgs e)
    {
        if (this.FilterTextBox!.Visibility != Visibility.Visible)
        {
            this.FilterTextBox.Visibility = Visibility.Visible;
            this.FilterTextBox.Text = string.Empty;
        }

        if (!this.FilterTextBox.IsKeyboardFocused)
        {
            this.FilterTextBox.Focus();
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

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var view = this._collectionViewSource.View;

        if (string.IsNullOrEmpty(this.FilterTextBox!.Text))
        {
            this.FilterTextBox.Visibility = Visibility.Collapsed;
            // Return focus to list so user can continue navigating immediately
            this.ListBox!.Focus();
            if (view != null && !view.IsEmpty)
            {
                if (this.ListBox.SelectedIndex < 0)
                {
                    view.MoveCurrentToFirst();
                    this.ListBox.SelectedIndex = 0;
                }
                else
                {
                    _ = view.MoveCurrentToPosition(this.ListBox.SelectedIndex);
                }
            }
        }

        this.UpdateFilterPredicate();
        view?.Refresh();
    }

    private void FilterTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.HandleEscape(e);
            return;
        }

        if (IsCtrlEdgeNavigation(e))
        {
            this.MoveSelectionToEdge(e.Key == Key.Up);
            e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
            this.MoveSelection(1);
            e.Handled = true;
        }
        else if (e.Key == Key.Up)
        {
            this.MoveSelection(-1);
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            if (this.ListBox!.SelectedIndex < 0 && this.ListBox.Items.Count > 0)
            {
                this.ListBox.SelectedIndex = 0;
            }
            if (this.ListBox.SelectedItem != null)
            {
                this.OnItemSelected();
            }
            e.Handled = true;
        }
    }

    private void HandleEscape(KeyEventArgs e)
    {
        // If text present, clear and swallow; if already empty, let popup handle (close)
        if (!string.IsNullOrEmpty(this.FilterTextBox!.Text))
        {
            this.FilterTextBox.Clear(); // triggers TextChanged -> collapse & focus list
            e.Handled = true; // prevent popup from closing
        }

        return;
    }

    private void ListBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        if (this.FilterTextBox!.Visibility != Visibility.Visible)
        {
            this.FilterTextBox.Visibility = Visibility.Visible;
            this.FilterTextBox.Text = string.Empty;
        }

        this.FilterTextBox.Text += e.Text;
        this.FilterTextBox.CaretIndex = this.FilterTextBox.Text.Length;
        this.FilterTextBox.Focus();

        e.Handled = true; // prevent ListBox text search
    }

    private void MoveSelection(int delta)
    {
        var view = this._collectionViewSource?.View;
        if (view == null || view.IsEmpty)
        {
            return;
        }

        if (view.CurrentItem == null)
        {
            view.MoveCurrentToFirst();
        }
        else
        {
            bool moved = delta > 0 ? view.MoveCurrentToNext() : view.MoveCurrentToPrevious();
            if (!moved)
            {
                if (delta > 0)
                {
                    view.MoveCurrentToFirst();
                }
                else
                {
                    view.MoveCurrentToLast();
                }
            }
        }

        var current = view.CurrentItem;
        if (current != null)
        {
            this.ListBox!.ScrollIntoView(current);
        }
    }

    private void MoveSelectionToEdge(bool toFirst)
    {
        var view = this._collectionViewSource?.View;
        if (view == null || view.IsEmpty)
        {
            return;
        }
        if (toFirst)
        {
            view.MoveCurrentToFirst();
        }
        else
        {
            view.MoveCurrentToLast();
        }
        var current = view.CurrentItem;
        if (current != null)
        {
            this.ListBox!.ScrollIntoView(current);
        }
    }

    private void UpdateFilterPredicate()
    {
        var view = this._collectionViewSource?.View;
        if (view == null)
        {
            return;
        }

        var filterText = this.FilterTextBox?.Text;
        if (string.IsNullOrWhiteSpace(filterText))
        {
            view.Filter = null;
        }
        else
        {
            view.Filter = this.FilterItem;
        }

        // After filter applied update placeholders
        this.UpdatePlaceholders();
    }

    private void UpdatePlaceholders()
    {
        if (this.ListBox == null)
        {
            return;
        }

        var sourceCollection = this._collectionViewSource.Source as System.Collections.IEnumerable;
        bool hasAnySourceItem = false;
        if (sourceCollection != null)
        {
            var enumerator = sourceCollection.GetEnumerator();
            if (enumerator.MoveNext())
            {
                hasAnySourceItem = true;
            }
        }

        var view = this._collectionViewSource.View;
        var hasVisibleItems = view != null && !view.IsEmpty;
        var filterActive = !string.IsNullOrWhiteSpace(this.FilterTextBox?.Text);

        if (!hasAnySourceItem)
        {
            // Nothing at all
            this.EmptyPlaceholder!.Visibility = Visibility.Visible;
            this.FilteredPlaceholder!.Visibility = Visibility.Collapsed;
        }
        else if (filterActive && !hasVisibleItems)
        {
            // Filter removed all
            this.EmptyPlaceholder!.Visibility = Visibility.Collapsed;
            this.FilteredPlaceholder!.Visibility = Visibility.Visible;
        }
        else
        {
            this.EmptyPlaceholder!.Visibility = Visibility.Collapsed;
            this.FilteredPlaceholder!.Visibility = Visibility.Collapsed;
        }
    }

    private bool FilterItem(object obj)
    {
        var filter = this.FilterTextBox?.Text;
        if (string.IsNullOrWhiteSpace(filter))
        {
            return true;
        }

        if (obj is SeparatorListItemViewModel)
        {
            return false;
        }

        if (obj is MemberListItemViewModel model)
        {
            var searchText = model.SearchText ?? model.PrimaryName ?? string.Empty;
            return searchText.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        return true;
    }
}