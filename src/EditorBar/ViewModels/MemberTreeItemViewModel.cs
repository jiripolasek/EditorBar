// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Collections.ObjectModel;
using System.Windows.Input;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.ViewModels;

public sealed class MemberTreeItemViewModel
{
    private bool _areChildrenLoaded;

    public StackedImageMoniker? ImageMoniker { get; init; }

    public required string PrimaryName { get; init; }

    public required string SearchText { get; init; }

    public ICommand? Command { get; init; }

    public object? CommandParameter { get; init; }

    public Func<Task<IList<MemberTreeItemViewModel>>>? ChildrenProvider { get; init; }

    public ObservableCollection<MemberTreeItemViewModel> Children { get; } = [];

    public bool AutoExpand { get; init; }

    public bool ExpandOnActivate { get; init; }

    public bool InvokeOnActivate { get; init; }

    internal bool IsPlaceholder { get; init; }

    internal bool AreChildrenLoaded => this._areChildrenLoaded;

    public bool CanHaveChildren => this.ChildrenProvider != null;

    public void PrepareForDisplay()
    {
        if (this.CanHaveChildren && !this._areChildrenLoaded && this.Children.Count == 0)
        {
            this.Children.Add(CreatePlaceholder());
        }
    }

    public async Task EnsureChildrenLoadedAsync()
    {
        if (!this.CanHaveChildren || this._areChildrenLoaded)
        {
            return;
        }

        var children = await this.ChildrenProvider!.Invoke();

        this.SetLoadedChildren(children);
    }

    internal void SetLoadedChildren(IEnumerable<MemberTreeItemViewModel> children)
    {
        this.Children.Clear();
        foreach (var child in children)
        {
            child.PrepareForDisplay();
            this.Children.Add(child);
        }

        this._areChildrenLoaded = true;
    }

    private static MemberTreeItemViewModel CreatePlaceholder()
    {
        return new MemberTreeItemViewModel
        {
            PrimaryName = "Loading...",
            SearchText = string.Empty,
            IsPlaceholder = true
        };
    }
}
