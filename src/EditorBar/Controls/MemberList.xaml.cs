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
        if (e.Key is Key.Enter or Key.Space)
        {
            this.OnItemSelected();
        }
        else if (IsTextInputKey(e))
        {
            this.ShowFilterAndForwardKey(e);
        }
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

        // Focus the filter box and let it handle the key
        if (!this.FilterTextBox.IsKeyboardFocused)
        {
            this.FilterTextBox.Focus();
        }

        // Do not mark handled here to allow text composition via PreviewTextInput
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
        if (string.IsNullOrEmpty(this.FilterTextBox!.Text))
        {
            this.FilterTextBox.Visibility = Visibility.Collapsed;
            // Return focus to list so user can continue navigating immediately
            this.ListBox!.Focus();
            if (this.ListBox.SelectedIndex < 0 && this.ListBox.Items.Count > 0)
            {
                this.ListBox.SelectedIndex = 0;
            }
        }

        this.UpdateFilterPredicate();
        this._collectionViewSource?.View?.Refresh();
    }

    private void FilterTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.FilterTextBox!.Text = string.Empty;
            this.FilterTextBox.Visibility = Visibility.Collapsed;
            this.UpdateFilterPredicate();
            this._collectionViewSource?.View?.Refresh();
            this.ListBox!.Focus();
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
            // Ensure something is selected; if not, select first filtered item
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

    private void ListBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        // When user starts typing while list has focus, show filter box and seed with the input
        if (this.FilterTextBox!.Visibility != Visibility.Visible)
        {
            this.FilterTextBox.Visibility = Visibility.Visible;
            this.FilterTextBox.Text = string.Empty;
        }

        // Append typed text and focus
        this.FilterTextBox.Text += e.Text;
        this.FilterTextBox.CaretIndex = this.FilterTextBox.Text.Length;
        this.FilterTextBox.Focus();

        e.Handled = true; // prevent ListBox text search
    }

    private void MoveSelection(int delta)
    {
        var list = this.ListBox!;
        var count = list.Items.Count;
        if (count == 0)
        {
            return;
        }

        var newIndex = list.SelectedIndex;
        if (newIndex < 0)
        {
            newIndex = delta > 0 ? 0 : count - 1;
        }
        else
        {
            newIndex = (newIndex + delta) % count;
            if (newIndex < 0)
            {
                newIndex += count; // wrap negative
            }
        }

        if (newIndex != list.SelectedIndex)
        {
            list.SelectedIndex = newIndex;
            list.ScrollIntoView(list.SelectedItem);
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
            // No filter -> show all
            view.Filter = null;
            return;
        }

        view.Filter = this.FilterItem;
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
            // Hide separators while filtering
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