// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Controls;

public partial class MemberTree : UserControl
{
    private static readonly TimeSpan FilterThrottleDelay = TimeSpan.FromMilliseconds(350);

    private readonly List<MemberTreeItemViewModel> _items;
    private readonly bool _rememberResultsViewPreference;
    private readonly bool _showFilterBoxWhenEmpty;
    private CancellationTokenSource? _filterDebounceCancellationTokenSource;
    private List<MemberTreeItemViewModel> _filteredListItems = [];
    private EventHandler? _pendingSelectFirstItemHandler;
    private EventHandler? _pendingSelectSpecificTreeItemHandler;
    private int _filterRequestVersion;
    private bool _preferListResultsView;
    private bool _suppressResultsViewToggleStateChange;

    public event EventHandler? ItemInvoked;

    public object? SelectedItem => this.IsListResultsViewActive ? this.ResultsListBox.SelectedItem : this.TreeView.SelectedItem;

    public MemberTree(IEnumerable<MemberTreeItemViewModel> items)
    {
        this.InitializeComponent();

        var options = GeneralOptionsModel.Instance;
        this._items = items.ToList();
        this._showFilterBoxWhenEmpty = options.ShowMemberListFilterBoxWhenEmpty;
        this._rememberResultsViewPreference = options.MemberTreeSearchResultViewDefault == MemberTreeSearchResultView.RememberLastUsed;
        this._preferListResultsView = options.MemberTreeSearchResultViewDefault switch
        {
            MemberTreeSearchResultView.List => true,
            MemberTreeSearchResultView.RememberLastUsed => options.LastUsedMemberTreeSearchResultViewIsList,
            _ => false
        };
        this.TreeView.ItemsSource = this._items;
        this.SyncResultsViewToggleState(this._preferListResultsView);
        this.ApplyEmptyFilterVisibilityPreference();
        this.UpdatePlaceholders(this._items.Count > 0);
    }

    private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
    {
        this.TreeViewItemOnExpandedAsync(e).FireAndForget();
    }

    private async Task TreeViewItemOnExpandedAsync(RoutedEventArgs e)
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
            this.UpdateResultsViewToggleVisibility(filterActive: false);
            this.SetResultsView(showList: false);
            this.ApplyEmptyFilterVisibilityPreference();
            this.FocusCurrentResultsView();
        }
    }

    private void FilterTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        this.FilterTextBoxOnKeyDownAsync(e).FireAndForget();
    }

    private async Task FilterTextBoxOnKeyDownAsync(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.HandleEscape(e);
            return;
        }

        if (e.Key == Key.Down)
        {
            this.FocusCurrentResultsView();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            await this.ActivateSelectedItemAsync();
            e.Handled = true;
        }
    }

    private void TreeView_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        this.TreeViewOnPreviewKeyDownAsync(e).FireAndForget();
    }

    private async Task TreeViewOnPreviewKeyDownAsync(KeyEventArgs e)
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
        else if (e.Key == Key.Apps || (e.Key == Key.F10 && Keyboard.Modifiers == ModifierKeys.Shift))
        {
            e.Handled = this.OpenContextMenuForSelectedItem();
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

    private void ResultsListBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        this.ResultsListBoxOnPreviewKeyDownAsync(e).FireAndForget();
    }

    private async Task ResultsListBoxOnPreviewKeyDownAsync(KeyEventArgs e)
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
        else if (e.Key == Key.Apps || (e.Key == Key.F10 && Keyboard.Modifiers == ModifierKeys.Shift))
        {
            e.Handled = this.OpenContextMenuForSelectedItem();
        }
        else if (IsTextInputKey(e))
        {
            this.ShowFilterAndForwardKey();
        }
    }

    private void ResultsListBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
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

    private void TreeViewItem_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 1)
        {
            return;
        }

        if (sender is not TreeViewItem { DataContext: MemberTreeItemViewModel item } container)
        {
            return;
        }

        if (!ReferenceEquals(this.FindNearestTreeViewItem(e.OriginalSource as DependencyObject), container))
        {
            return;
        }

        if (this.FindNearestAncestor<ToggleButton>(e.OriginalSource as DependencyObject) != null)
        {
            return;
        }

        e.Handled = true;
        container.IsSelected = true;
        this.ActivateItemAsync(item).FireAndForget();
    }

    private void TreeViewItem_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TreeViewItem { DataContext: MemberTreeItemViewModel item } container)
        {
            return;
        }

        if (!ReferenceEquals(this.FindNearestTreeViewItem(e.OriginalSource as DependencyObject), container))
        {
            return;
        }

        if (!this.OpenContextMenu(item, container))
        {
            return;
        }

        e.Handled = true;
    }

    private async Task ActivateSelectedItemAsync()
    {
        var selectedItem = this.IsListResultsViewActive
            ? this.ResultsListBox.SelectedItem
            : this.TreeView.SelectedItem;

        if (selectedItem is not MemberTreeItemViewModel { IsPlaceholder: false } selectedTreeItem)
        {
            return;
        }

        await this.ActivateItemAsync(selectedTreeItem);
    }

    private async Task ActivateItemAsync(MemberTreeItemViewModel item)
    {
        if (item.IsPlaceholder)
        {
            return;
        }

        if (this.IsListResultsViewActive)
        {
            if (item.CanHaveChildren && item.ExpandOnActivate)
            {
                this.SyncResultsViewToggleState(isChecked: false);
                this.SelectTreeItem(item, expand: true);
                return;
            }

            this.ItemInvoked?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (item.CanHaveChildren && item.ExpandOnActivate)
        {
            if (this.FindContainer(this.TreeView, item) is { } container)
            {
                container.IsSelected = true;
                if (!container.IsExpanded)
                {
                    await item.EnsureChildrenLoadedAsync();
                }

                container.IsExpanded = !container.IsExpanded;
            }

            return;
        }

        this.ItemInvoked?.Invoke(this, EventArgs.Empty);
    }

    private bool OpenContextMenuForSelectedItem()
    {
        if (this.IsListResultsViewActive)
        {
            if (this.ResultsListBox.SelectedItem is not MemberTreeItemViewModel selectedListItem || selectedListItem.IsPlaceholder)
            {
                return false;
            }

            return this.OpenContextMenu(selectedListItem, this.FindListContainer(selectedListItem));
        }

        if (this.TreeView.SelectedItem is not MemberTreeItemViewModel selectedItem || selectedItem.IsPlaceholder)
        {
            return false;
        }

        return this.OpenContextMenu(selectedItem, this.FindContainer(this.TreeView, selectedItem));
    }

    private bool OpenContextMenu(MemberTreeItemViewModel item, TreeViewItem? container)
    {
        if (item.IsPlaceholder || container == null || item.ContextCommand == null || !item.ContextCommand.CanExecute(null))
        {
            return false;
        }

        container.IsSelected = true;
        item.ContextCommand.Execute(null);
        return true;
    }

    private bool OpenContextMenu(MemberTreeItemViewModel item, ListBoxItem? container)
    {
        if (item.IsPlaceholder || container == null || item.ContextCommand == null || !item.ContextCommand.CanExecute(null))
        {
            return false;
        }

        container.IsSelected = true;
        item.ContextCommand.Execute(null);
        return true;
    }

    private async Task ApplyFilterWithDebounceAsync(int version, CancellationToken cancellationToken)
    {
        var filter = this.FilterTextBox.Text;
        if (!string.IsNullOrWhiteSpace(filter))
        {
            try
            {
                await Task.Delay(FilterThrottleDelay, cancellationToken);
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
            this._filteredListItems = [];
            this.ResultsListBox.ItemsSource = this._filteredListItems;
            this.UpdatePlaceholders(this._items.Count > 0);
            this.UpdateResultsViewToggleVisibility(filterActive: false);
            this.SetResultsView(showList: false);
            this.SelectFirstItem();
            return;
        }

        var matcher = new SearchPatternMatcher(filter);
        var filteredItems = new List<MemberTreeItemViewModel>();
        foreach (var item in this._items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filteredItem = await this.CreateFilteredNodeAsync(item, matcher, cancellationToken);
            if (filteredItem != null)
            {
                filteredItems.Add(filteredItem);
            }
        }

        var flatListItems = FlattenMatches(filteredItems, matcher);

        if (!this.IsCurrentFilterRequest(version, cancellationToken))
        {
            return;
        }

        this.TreeView.ItemsSource = filteredItems;
        this._filteredListItems = flatListItems;
        this.ResultsListBox.ItemsSource = this._filteredListItems;
        this.UpdateResultsViewToggleVisibility(filterActive: true);
        this.SyncResultsViewToggleState(this._preferListResultsView);
        this.SetResultsView(this._preferListResultsView);
        this.UpdatePlaceholders(this.IsListResultsViewActive ? this._filteredListItems.Count > 0 : filteredItems.Count > 0);
        this.SelectFirstItem();
    }

    private async Task<MemberTreeItemViewModel?> CreateFilteredNodeAsync(
        MemberTreeItemViewModel item,
        SearchPatternMatcher matcher,
        CancellationToken cancellationToken)
    {
        var matchesSelf = this.MatchesFilter(item, matcher);
        var filteredChildren = new List<MemberTreeItemViewModel>();

        if (item.CanHaveChildren)
        {
            await item.EnsureChildrenLoadedAsync();
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var child in item.Children.Where(static child => !child.IsPlaceholder))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var filteredChild = await this.CreateFilteredNodeAsync(child, matcher, cancellationToken);
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
            SecondaryName = item.SecondaryName,
            ImageMoniker = item.ImageMoniker,
            Command = item.Command,
            CommandParameter = item.CommandParameter,
            ContextCommand = item.ContextCommand,
            ExpandOnActivate = item.ExpandOnActivate,
            InvokeOnActivate = item.InvokeOnActivate,
            ChildrenProvider = filteredChildren.Count > 0
                ? () => Task.FromResult<IList<MemberTreeItemViewModel>>(filteredChildren)
                : null,
            AutoExpand = filteredChildren.Count > 0 && !matchesSelf
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

    private static List<MemberTreeItemViewModel> FlattenMatches(
        IEnumerable<MemberTreeItemViewModel> treeItems,
        SearchPatternMatcher matcher)
    {
        var matches = new List<MemberTreeItemViewModel>();
        foreach (var item in treeItems)
        {
            CollectMatches(item, matcher, matches);
        }

        return matches;
    }

    private static void CollectMatches(
        MemberTreeItemViewModel item,
        SearchPatternMatcher matcher,
        ICollection<MemberTreeItemViewModel> matches)
    {
        if (item.IsPlaceholder)
        {
            return;
        }

        var searchText = string.IsNullOrWhiteSpace(item.SearchText) ? item.PrimaryName : item.SearchText;
        if (matcher.IsMatch(searchText))
        {
            matches.Add(item);
        }

        foreach (var child in item.Children.Where(static child => !child.IsPlaceholder))
        {
            CollectMatches(child, matcher, matches);
        }
    }

    private bool MatchesFilter(MemberTreeItemViewModel item, SearchPatternMatcher matcher)
    {
        var searchText = string.IsNullOrWhiteSpace(item.SearchText) ? item.PrimaryName : item.SearchText;
        return matcher.IsMatch(searchText);
    }

    private void SelectFirstItem()
    {
        if (this.IsListResultsViewActive)
        {
            this.SelectFirstListItem();
            return;
        }

        if (this.TreeView.Items.Count == 0)
        {
            return;
        }

        if (this.TrySelectFirstItem())
        {
            return;
        }

        if (this._pendingSelectFirstItemHandler != null)
        {
            this.TreeView.LayoutUpdated -= this._pendingSelectFirstItemHandler;
        }

        this._pendingSelectFirstItemHandler = (_, _) =>
        {
            if (!this.TrySelectFirstItem())
            {
                return;
            }

            if (this._pendingSelectFirstItemHandler != null)
            {
                this.TreeView.LayoutUpdated -= this._pendingSelectFirstItemHandler;
                this._pendingSelectFirstItemHandler = null;
            }
        };

        this.TreeView.LayoutUpdated += this._pendingSelectFirstItemHandler;
    }

    private void SelectFirstListItem()
    {
        if (this.ResultsListBox.Items.Count == 0)
        {
            return;
        }

        if (this.TrySelectFirstListItem())
        {
            return;
        }

        if (this._pendingSelectFirstItemHandler != null)
        {
            this.ResultsListBox.LayoutUpdated -= this._pendingSelectFirstItemHandler;
        }

        this._pendingSelectFirstItemHandler = (_, _) =>
        {
            if (!this.TrySelectFirstListItem())
            {
                return;
            }

            if (this._pendingSelectFirstItemHandler != null)
            {
                this.ResultsListBox.LayoutUpdated -= this._pendingSelectFirstItemHandler;
                this._pendingSelectFirstItemHandler = null;
            }
        };

        this.ResultsListBox.LayoutUpdated += this._pendingSelectFirstItemHandler;
    }

    private bool TrySelectFirstItem()
    {
        if (this.TreeView.ItemContainerGenerator.ContainerFromIndex(0) is not TreeViewItem firstItem)
        {
            return false;
        }

        firstItem.IsSelected = true;
        if (this.FilterTextBox.Visibility == Visibility.Visible && this.FilterTextBox.IsKeyboardFocused)
        {
            return true;
        }

        firstItem.Focus();
        return true;
    }

    private bool TrySelectFirstListItem()
    {
        if (this.ResultsListBox.ItemContainerGenerator.ContainerFromIndex(0) is not ListBoxItem firstItem)
        {
            return false;
        }

        firstItem.IsSelected = true;
        if (this.FilterTextBox.Visibility == Visibility.Visible && this.FilterTextBox.IsKeyboardFocused)
        {
            return true;
        }

        firstItem.Focus();
        return true;
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

    private TreeViewItem? FindNearestTreeViewItem(DependencyObject? source)
    {
        var current = source;
        while (current != null)
        {
            if (current is TreeViewItem treeViewItem)
            {
                return treeViewItem;
            }

            current = VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current);
        }

        return null;
    }

    private T? FindNearestAncestor<T>(DependencyObject? source)
        where T : DependencyObject
    {
        var current = source;
        while (current != null)
        {
            if (current is T match)
            {
                return match;
            }

            current = VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current);
        }

        return null;
    }

    private void ApplyEmptyFilterVisibilityPreference()
    {
        this.FilterTextBox.Visibility = this._showFilterBoxWhenEmpty
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void ResultsViewToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        if (this._suppressResultsViewToggleStateChange)
        {
            return;
        }

        this.UpdatePreferredResultsView(preferList: true);
        this.SetResultsView(showList: true);
        this.SelectFirstItem();
    }

    private void ResultsViewToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (this._suppressResultsViewToggleStateChange)
        {
            return;
        }

        this.UpdatePreferredResultsView(preferList: false);
        this.SetResultsView(showList: false);
        this.SelectFirstItem();
    }

    private void UpdateResultsViewToggleVisibility(bool filterActive)
    {
        if (!filterActive)
        {
            this.ResultsViewToggleButton.Visibility = Visibility.Collapsed;
            return;
        }

        this.ResultsViewToggleButton.Visibility = Visibility.Visible;
    }

    private void SetResultsView(bool showList)
    {
        var canShowList = showList &&
                          !string.IsNullOrWhiteSpace(this.FilterTextBox.Text) &&
                          this._filteredListItems.Count > 0;

        this.TreeView.Visibility = canShowList ? Visibility.Collapsed : Visibility.Visible;
        this.ResultsListBox.Visibility = canShowList ? Visibility.Visible : Visibility.Collapsed;
    }

    private bool IsListResultsViewActive => this.ResultsListBox.Visibility == Visibility.Visible;

    private void FocusCurrentResultsView()
    {
        if (this.IsListResultsViewActive)
        {
            this.ResultsListBox.Focus();
            return;
        }

        this.TreeView.Focus();
    }

    private void SelectTreeItem(MemberTreeItemViewModel item, bool expand)
    {
        this.SyncResultsViewToggleState(isChecked: false);
        this.SetResultsView(showList: false);

        if (this.FindContainer(this.TreeView, item) is { } container)
        {
            this.SelectTreeContainer(container, expand);
            return;
        }

        if (this._pendingSelectSpecificTreeItemHandler != null)
        {
            this.TreeView.LayoutUpdated -= this._pendingSelectSpecificTreeItemHandler;
        }

        this._pendingSelectSpecificTreeItemHandler = (_, _) =>
        {
            if (this.FindContainer(this.TreeView, item) is not { } pendingContainer)
            {
                return;
            }

            this.SelectTreeContainer(pendingContainer, expand);
            if (this._pendingSelectSpecificTreeItemHandler != null)
            {
                this.TreeView.LayoutUpdated -= this._pendingSelectSpecificTreeItemHandler;
                this._pendingSelectSpecificTreeItemHandler = null;
            }
        };

        this.TreeView.LayoutUpdated += this._pendingSelectSpecificTreeItemHandler;
    }

    private void SelectTreeContainer(TreeViewItem container, bool expand)
    {
        container.IsSelected = true;
        if (expand)
        {
            container.IsExpanded = true;
        }

        if (this.FilterTextBox.Visibility == Visibility.Visible && this.FilterTextBox.IsKeyboardFocused)
        {
            return;
        }

        container.Focus();
    }

    private void ResultsListBoxItem_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 1)
        {
            return;
        }

        if (sender is not ListBoxItem { DataContext: MemberTreeItemViewModel item } container)
        {
            return;
        }

        if (!ReferenceEquals(this.FindNearestAncestor<ListBoxItem>(e.OriginalSource as DependencyObject), container))
        {
            return;
        }

        e.Handled = true;
        container.IsSelected = true;
        this.ActivateItemAsync(item).FireAndForget();
    }

    private void ResultsListBoxItem_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListBoxItem { DataContext: MemberTreeItemViewModel item } container)
        {
            return;
        }

        if (!ReferenceEquals(this.FindNearestAncestor<ListBoxItem>(e.OriginalSource as DependencyObject), container))
        {
            return;
        }

        if (!this.OpenContextMenu(item, container))
        {
            return;
        }

        e.Handled = true;
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

    private ListBoxItem? FindListContainer(object item)
    {
        return this.ResultsListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
    }

    private void SyncResultsViewToggleState(bool isChecked)
    {
        this._suppressResultsViewToggleStateChange = true;
        this.ResultsViewToggleButton.IsChecked = isChecked;
        this._suppressResultsViewToggleStateChange = false;
    }

    private void UpdatePreferredResultsView(bool preferList)
    {
        this._preferListResultsView = preferList;
        if (!this._rememberResultsViewPreference)
        {
            return;
        }

        var options = GeneralOptionsModel.Instance;
        if (options.LastUsedMemberTreeSearchResultViewIsList == preferList)
        {
            return;
        }

        options.LastUsedMemberTreeSearchResultViewIsList = preferList;
        options.Save();
    }
}
