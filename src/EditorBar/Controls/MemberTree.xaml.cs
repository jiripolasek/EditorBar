// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Controls;

public partial class MemberTree : UserControl
{
    private readonly List<MemberTreeItemViewModel> _items;
    private readonly bool _showFilterBoxWhenEmpty;
    private CancellationTokenSource? _filterDebounceCancellationTokenSource;
    private int _filterRequestVersion;

    public event EventHandler? ItemInvoked;

    public object? SelectedItem => this.TreeView.SelectedItem;

    public MemberTree(IEnumerable<MemberTreeItemViewModel> items)
    {
        this.InitializeComponent();

        this._items = items.ToList();
        this._showFilterBoxWhenEmpty = GeneralOptionsModel.Instance.ShowMemberListFilterBoxWhenEmpty;
        this.TreeView.ItemsSource = this._items;
        this.ApplyEmptyFilterVisibilityPreference();
        this.UpdatePlaceholders(this._items.Count > 0);
    }

    private async void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not TreeViewItem { DataContext: MemberTreeItemViewModel item } || item.IsPlaceholder)
        {
            return;
        }

        await item.EnsureChildrenLoadedAsync();
    }

    private void TreeViewItem_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is TreeViewItem { DataContext: MemberTreeItemViewModel { AutoExpand: true } } treeViewItem)
        {
            treeViewItem.IsExpanded = true;
        }
    }

    private void TreeView_OnLoaded(object sender, RoutedEventArgs e)
    {
        this.SelectFirstItem();
    }

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        this._filterDebounceCancellationTokenSource?.Cancel();
        this._filterDebounceCancellationTokenSource?.Dispose();
        this._filterDebounceCancellationTokenSource = new CancellationTokenSource();
        var version = ++this._filterRequestVersion;
        _ = this.ApplyFilterWithDebounceAsync(version, this._filterDebounceCancellationTokenSource.Token);

        if (string.IsNullOrEmpty(this.FilterTextBox.Text))
        {
            this.ApplyEmptyFilterVisibilityPreference();
            this.TreeView.Focus();
        }
    }

    private async void FilterTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.HandleEscape(e);
            return;
        }

        if (e.Key == Key.Down)
        {
            this.TreeView.Focus();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            await this.ActivateSelectedItemAsync();
            e.Handled = true;
        }
    }

    private async void TreeView_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.HandleEscape(e);
        }
        else if (this.ShouldIgnoreHiddenEmptyFilterDeletion(e))
        {
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            await this.ActivateSelectedItemAsync();
            e.Handled = true;
        }
        else if (IsTextInputKey(e))
        {
            this.ShowFilterAndForwardKey();
        }
    }

    private void TreeView_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        this.ShowFilterAndForwardKey();
        this.FilterTextBox.Text += e.Text;
        this.FilterTextBox.CaretIndex = this.FilterTextBox.Text.Length;
        e.Handled = true;
    }

    private async void TreeViewItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TreeViewItem)
        {
            return;
        }

        await this.ActivateSelectedItemAsync();
        e.Handled = true;
    }

    private async Task ActivateSelectedItemAsync()
    {
        if (this.TreeView.SelectedItem is not MemberTreeItemViewModel selectedItem || selectedItem.IsPlaceholder)
        {
            return;
        }

        if (selectedItem.CanHaveChildren && !selectedItem.InvokeOnActivate)
        {
            if (this.FindContainer(this.TreeView, selectedItem) is { } container)
            {
                if (!container.IsExpanded)
                {
                    await selectedItem.EnsureChildrenLoadedAsync();
                }

                container.IsExpanded = !container.IsExpanded;
            }

            return;
        }

        this.ItemInvoked?.Invoke(this, EventArgs.Empty);
    }

    private async Task ApplyFilterWithDebounceAsync(int version, CancellationToken cancellationToken)
    {
        var filter = this.FilterTextBox.Text;
        if (!string.IsNullOrWhiteSpace(filter))
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(175), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        await this.ApplyFilterAsync(version, cancellationToken);
    }

    private async Task ApplyFilterAsync(int version, CancellationToken cancellationToken)
    {
        var filter = this.FilterTextBox.Text;
        if (string.IsNullOrWhiteSpace(filter))
        {
            if (!this.IsCurrentFilterRequest(version, cancellationToken))
            {
                return;
            }

            this.TreeView.ItemsSource = this._items;
            this.UpdatePlaceholders(this._items.Count > 0);
            this.SelectFirstItem();
            return;
        }

        var filteredItems = new List<MemberTreeItemViewModel>();
        foreach (var item in this._items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filteredItem = await this.CreateFilteredNodeAsync(item, filter, cancellationToken);
            if (filteredItem != null)
            {
                filteredItems.Add(filteredItem);
            }
        }

        if (!this.IsCurrentFilterRequest(version, cancellationToken))
        {
            return;
        }

        this.TreeView.ItemsSource = filteredItems;
        this.UpdatePlaceholders(filteredItems.Count > 0);
        this.SelectFirstItem();
    }

    private async Task<MemberTreeItemViewModel?> CreateFilteredNodeAsync(
        MemberTreeItemViewModel item,
        string filter,
        CancellationToken cancellationToken)
    {
        var matchesSelf = this.MatchesFilter(item, filter);
        var filteredChildren = new List<MemberTreeItemViewModel>();

        if (item.CanHaveChildren)
        {
            await item.EnsureChildrenLoadedAsync();
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var child in item.Children.Where(static child => !child.IsPlaceholder))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var filteredChild = await this.CreateFilteredNodeAsync(child, filter, cancellationToken);
                if (filteredChild != null)
                {
                    filteredChildren.Add(filteredChild);
                }
            }
        }

        if (!matchesSelf && filteredChildren.Count == 0)
        {
            return null;
        }

        var clone = new MemberTreeItemViewModel
        {
            PrimaryName = item.PrimaryName,
            SearchText = item.SearchText,
            ImageMoniker = item.ImageMoniker,
            Command = item.Command,
            CommandParameter = item.CommandParameter,
            ChildrenProvider = filteredChildren.Count > 0
                ? () => Task.FromResult<IList<MemberTreeItemViewModel>>(filteredChildren)
                : null,
            AutoExpand = filteredChildren.Count > 0
        };

        if (filteredChildren.Count > 0)
        {
            clone.SetLoadedChildren(filteredChildren);
        }

        return clone;
    }

    private bool IsCurrentFilterRequest(int version, CancellationToken cancellationToken)
    {
        return !cancellationToken.IsCancellationRequested && version == this._filterRequestVersion;
    }

    private bool MatchesFilter(MemberTreeItemViewModel item, string filter)
    {
        var searchText = string.IsNullOrWhiteSpace(item.SearchText) ? item.PrimaryName : item.SearchText;
        return searchText.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    private void SelectFirstItem()
    {
        if (this.TreeView.Items.Count == 0)
        {
            return;
        }

        this.Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            new Action(() =>
            {
                if (this.TreeView.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem firstItem)
                {
                    firstItem.IsSelected = true;
                    if (this.FilterTextBox.Visibility == Visibility.Visible && this.FilterTextBox.IsKeyboardFocused)
                    {
                        return;
                    }

                    firstItem.Focus();
                }
            }));
    }

    private void UpdatePlaceholders(bool hasVisibleItems)
    {
        var filterActive = !string.IsNullOrWhiteSpace(this.FilterTextBox.Text);
        if (this._items.Count == 0)
        {
            this.EmptyPlaceholder.Visibility = Visibility.Visible;
            this.FilteredPlaceholder.Visibility = Visibility.Collapsed;
        }
        else if (filterActive && !hasVisibleItems)
        {
            this.EmptyPlaceholder.Visibility = Visibility.Collapsed;
            this.FilteredPlaceholder.Visibility = Visibility.Visible;
        }
        else
        {
            this.EmptyPlaceholder.Visibility = Visibility.Collapsed;
            this.FilteredPlaceholder.Visibility = Visibility.Collapsed;
        }
    }

    private void HandleEscape(KeyEventArgs e)
    {
        if (!string.IsNullOrEmpty(this.FilterTextBox.Text))
        {
            this.FilterTextBox.Clear();
            e.Handled = true;
        }
    }

    private void ShowFilterAndForwardKey()
    {
        if (this.FilterTextBox.Visibility != Visibility.Visible)
        {
            this.FilterTextBox.Visibility = Visibility.Visible;
            this.FilterTextBox.Text = string.Empty;
        }

        if (!this.FilterTextBox.IsKeyboardFocused)
        {
            this.FilterTextBox.Focus();
        }
    }

    private static bool IsTextInputKey(KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
            e.Key == Key.LeftShift || e.Key == Key.RightShift ||
            e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
            e.Key == Key.Tab || e.Key == Key.Escape ||
            e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
        {
            return false;
        }

        return e.Key == Key.Back || e.Key == Key.Delete || Keyboard.Modifiers == ModifierKeys.None;
    }

    private bool ShouldIgnoreHiddenEmptyFilterDeletion(KeyEventArgs e)
    {
        return this.FilterTextBox.Visibility != Visibility.Visible &&
               string.IsNullOrEmpty(this.FilterTextBox.Text) &&
               e.Key is Key.Back or Key.Delete;
    }

    private void ApplyEmptyFilterVisibilityPreference()
    {
        this.FilterTextBox.Visibility = this._showFilterBoxWhenEmpty
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private TreeViewItem? FindContainer(ItemsControl parent, object item)
    {
        if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem directMatch)
        {
            return directMatch;
        }

        foreach (var child in parent.Items.Cast<object>())
        {
            if (parent.ItemContainerGenerator.ContainerFromItem(child) is not TreeViewItem childContainer)
            {
                continue;
            }

            var nestedMatch = this.FindContainer(childContainer, item);
            if (nestedMatch != null)
            {
                return nestedMatch;
            }
        }

        return null;
    }
}
