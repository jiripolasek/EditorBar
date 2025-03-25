// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers.Events.Abstractions;

namespace JPSoftworks.EditorBar.Helpers.Events;

/// <summary>
/// Aggregates solution and project-level changes (like project rename,
/// item addition/removal, solution open) that may affect file breadcrumbs.
/// </summary>
internal sealed class SolutionProjectChangeEventAggregator : IDisposable
{
    /// <summary>
    /// Raised whenever a solution/project event occurs that might change the current file breadcrumbs.
    /// </summary>
    public event EventHandler<SolutionProjectChangedEventArgs>? Changed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionProjectChangeEventAggregator" /> class.
    /// </summary>
    public SolutionProjectChangeEventAggregator()
    {
        // Subscribe to solution events
        VS.Events.SolutionEvents.OnAfterRenameProject += this.OnProjectRenamed;
        VS.Events.SolutionEvents.OnAfterOpenSolution += this.OnSolutionOpened;

        // Subscribe to project item events
        VS.Events.ProjectItemsEvents.AfterRenameProjectItems += this.OnProjectItemRenamed;
        VS.Events.ProjectItemsEvents.AfterAddProjectItems += this.OnProjectItemAdded;
        VS.Events.ProjectItemsEvents.AfterRemoveProjectItems += this.OnProjectItemRemoved;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Unsubscribe from solution events
        VS.Events.SolutionEvents.OnAfterRenameProject -= this.OnProjectRenamed;
        VS.Events.SolutionEvents.OnAfterOpenSolution -= this.OnSolutionOpened;

        // Unsubscribe from project item events
        VS.Events.ProjectItemsEvents.AfterRenameProjectItems -= this.OnProjectItemRenamed;
        VS.Events.ProjectItemsEvents.AfterAddProjectItems -= this.OnProjectItemAdded;
        VS.Events.ProjectItemsEvents.AfterRemoveProjectItems -= this.OnProjectItemRemoved;
    }

    private void OnProjectRenamed(Project? project)
    {
        this.RaiseChanged(SolutionProjectChangeReason.ProjectRenamed);
    }

    private void OnSolutionOpened(Solution? solution)
    {
        this.RaiseChanged(SolutionProjectChangeReason.SolutionOpened);
    }

    private void OnProjectItemRenamed(AfterRenameProjectItemEventArgs? args)
    {
        this.RaiseChanged(SolutionProjectChangeReason.ProjectItemRenamed);
    }

    private void OnProjectItemAdded(IEnumerable<SolutionItem> items)
    {
        this.RaiseChanged(SolutionProjectChangeReason.ProjectItemAdded);
    }

    private void OnProjectItemRemoved(AfterRemoveProjectItemEventArgs? args)
    {
        this.RaiseChanged(SolutionProjectChangeReason.ProjectItemRemoved);
    }

    private void RaiseChanged(SolutionProjectChangeReason reason)
    {
        this.Changed?.Invoke(this, new SolutionProjectChangedEventArgs(reason));
    }
}