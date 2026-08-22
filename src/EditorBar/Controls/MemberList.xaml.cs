// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JPSoftworks.EditorBar.Commands;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for MemberList.xaml
/// </summary>
public partial class MemberList : UserControl
{
    public event EventHandler? ItemSelected;

    private readonly CollectionViewSource _collectionViewSource;
    private SearchPatternMatcher? _filterMatcher;
    private bool _showFilterBoxWhenEmpty;

    public MemberList()
    {
        this.InitializeComponent();
        this._showFilterBoxWhenEmpty = GeneralOptionsModel.Instance.ShowMemberListFilterBoxWhenEmpty;
        this._collectionViewSource = new CollectionViewSource();
        this.ListBox!.ItemsSource = this._collectionViewSource.View;
        this.ApplyEmptyFilterVisibilityPreference();
        this.UpdateFilterPredicate();
    }

    public MemberList(IEnumerable<MemberListItemViewModel> members)
        : this()
    {
        this._collectionViewSource.Source = members;
        this.ListBox!.ItemsSource = this._collectionViewSource.View;
        this.UpdateFilterPredicate();
    }

    public MemberListPopup? PopupHost { get; set; }

    internal bool ShowFilterBoxWhenEmpty => this._showFilterBoxWhenEmpty;

    private void ListBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape)
        {
            this.HandleEscape(e);
        }
        else if (this.ShouldIgnoreHiddenEmptyFilterDeletion(e))
        {
            e.Handled = true;
        }
        else if (e.Key is Key.Enter or Key.Space)
        {
            this.OnItemSelected();
            e.Handled = true;
        }
        else if (e.Key == Key.Apps || (e.Key == Key.F10 && Keyboard.Modifiers == ModifierKeys.Shift))
        {
            e.Handled = this.OpenContextMenuForSelectedItem();
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
        return (e.Key == Key.Up || e.Key == Key.Down) &&
               (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
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
        if (this.ShouldIgnoreHiddenEmptyFilterDeletion(e))
        {
            return;
        }

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

    private void ListBoxItem_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListBoxItem { DataContext: MemberListItemViewModel item } container ||
            !this.OpenContextMenu(item, container))
        {
            return;
        }

        e.Handled = true;
    }

    private bool OpenContextMenuForSelectedItem()
    {
        if (this.ListBox?.SelectedItem is not MemberListItemViewModel item ||
            this.ListBox.ItemContainerGenerator.ContainerFromItem(item) is not ListBoxItem container)
        {
            return false;
        }

        return this.OpenContextMenu(item, container);
    }

    private bool OpenContextMenu(MemberListItemViewModel item, ListBoxItem container)
    {
        if (item.ContextCommand == null || !item.ContextCommand.CanExecute(null))
        {
            return false;
        }

        container.IsSelected = true;
        using var popupMenuScope = this.PopupHost != null
            ? MemberListPopup.EnterMenuInteraction(this.PopupHost)
            : null;
        item.ContextCommand.Execute(null);
        return true;
    }

    private void ListBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        this.SelectFirstItemForFocusedList();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler")]
    private async void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var view = this._collectionViewSource.View;
            var shouldReturnFocusToList = string.IsNullOrEmpty(this.FilterTextBox!.Text);

            if (shouldReturnFocusToList)
            {
                this.ApplyEmptyFilterVisibilityPreference();
            }

            this.UpdateFilterPredicate();
            view?.Refresh();

            if (shouldReturnFocusToList)
            {
                // Return focus only after the refreshed view is in place, otherwise selection can be cleared again.
                await Task.Yield();
                this.ListBox!.Focus();
                this.SelectFirstItemForFocusedList();
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }
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
        else if (e.Key == Key.Apps || (e.Key == Key.F10 && Keyboard.Modifiers == ModifierKeys.Shift))
        {
            e.Handled = this.OpenContextMenuForSelectedItem();
        }
    }

    private void MoreOptionsButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        using var popupMenuScope = this.PopupHost != null
            ? MemberListPopup.EnterMenuInteraction(this.PopupHost)
            : null;

        new MemberListOptionsMenuContext(this, GetMenuLocation(button)).ShowMenu();
        e.Handled = true;
    }

    private void HandleEscape(KeyEventArgs e)
    {
        // If text present, clear and swallow; if already empty, let popup handle (close)
        if (!string.IsNullOrEmpty(this.FilterTextBox!.Text))
        {
            this.FilterTextBox.Clear(); // triggers TextChanged -> collapse & focus list
            e.Handled = true; // prevent popup from closing
        }
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

        if (this.FilterTextBox?.Text is not { Length: > 0 } filterText)
        {
            this._filterMatcher = null;
            view.Filter = null;
        }
        else
        {
            this._filterMatcher = new SearchPatternMatcher(filterText);
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

        var sourceCollection = this._collectionViewSource.Source as IEnumerable;
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
        if (this._filterMatcher == null)
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
            return this._filterMatcher.IsMatch(searchText);
        }

        return true;
    }

    private bool ShouldIgnoreHiddenEmptyFilterDeletion(KeyEventArgs e)
    {
        return this.FilterTextBox!.Visibility != Visibility.Visible &&
               string.IsNullOrEmpty(this.FilterTextBox.Text) &&
               e.Key is Key.Back or Key.Delete;
    }

    private void ApplyEmptyFilterVisibilityPreference()
    {
        if (this.FilterTextBox == null)
        {
            return;
        }

        this.FilterTextBox.Visibility = this._showFilterBoxWhenEmpty
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    internal void SetShowFilterBoxWhenEmpty(bool showFilterBoxWhenEmpty)
    {
        if (this._showFilterBoxWhenEmpty == showFilterBoxWhenEmpty)
        {
            return;
        }

        var shouldMoveFocusToList = !showFilterBoxWhenEmpty &&
                                    string.IsNullOrEmpty(this.FilterTextBox?.Text) &&
                                    this.FilterTextBox?.IsKeyboardFocusWithin == true;

        this._showFilterBoxWhenEmpty = showFilterBoxWhenEmpty;

        var options = GeneralOptionsModel.Instance;
        if (options.ShowMemberListFilterBoxWhenEmpty != showFilterBoxWhenEmpty)
        {
            options.ShowMemberListFilterBoxWhenEmpty = showFilterBoxWhenEmpty;
            options.Save();
        }

        if (string.IsNullOrEmpty(this.FilterTextBox?.Text))
        {
            this.ApplyEmptyFilterVisibilityPreference();
        }

        if (shouldMoveFocusToList)
        {
            this.ListBox?.Focus();
            this.SelectFirstItemForFocusedList();
        }
    }

    private void SelectFirstItemForFocusedList()
    {
        var view = this._collectionViewSource.View;
        if (view == null || view.IsEmpty || this.ListBox == null)
        {
            return;
        }

        for (var i = 0; i < this.ListBox.Items.Count; i++)
        {
            if (this.ListBox.Items[i] is SeparatorListItemViewModel)
            {
                continue;
            }

            this.ListBox.SelectedIndex = i;
            _ = view.MoveCurrentToPosition(i);
            this.ListBox.ScrollIntoView(this.ListBox.Items[i]);
            return;
        }
    }

    private static System.Drawing.Point GetMenuLocation(FrameworkElement element)
    {
        var screenPoint = element.PointToScreen(new Point(0, element.ActualHeight));
        return new System.Drawing.Point((int)Math.Round(screenPoint.X), (int)Math.Round(screenPoint.Y));
    }
}
