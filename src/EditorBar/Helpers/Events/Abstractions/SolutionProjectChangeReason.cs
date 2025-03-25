// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers.Events.Abstractions;

/// <summary>
/// Enumerates the types of solution/project changes that might affect breadcrumb paths.
/// </summary>
public enum SolutionProjectChangeReason
{
    /// <summary>
    /// Indicates that a project has been renamed.
    /// </summary>
    ProjectRenamed,

    /// <summary>
    /// Indicates that a project item has been renamed.
    /// </summary>
    ProjectItemRenamed,

    /// <summary>
    /// Indicates that a new project item has been added.
    /// </summary>
    ProjectItemAdded,

    /// <summary>
    /// Indicates that an existing project item has been removed.
    /// </summary>
    ProjectItemRemoved,

    /// <summary>
    /// Indicates that a solution has been opened.
    /// </summary>
    SolutionOpened
}