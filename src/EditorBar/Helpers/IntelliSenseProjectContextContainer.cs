// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Represents a container for IntelliSense project context, holding alternative documents and an active project.
/// </summary>
public record struct IntelliSenseProjectContextContainer
{
    /// <summary>
    /// Represents an empty IntelliSense project context container.
    /// </summary>
    public static readonly IntelliSenseProjectContextContainer Empty = new([], null);

    /// <summary>Holds a list of documents that provide additional context for IntelliSense features.</summary>
    public IReadOnlyList<Document> AlternativeContextDocuments { get; }

    /// <summary>Represents the currently active project within the IntelliSense context.</summary>
    public Project? ActiveProject { get; }

    /// <summary>
    /// Represents a container for IntelliSense project context, holding alternative documents and an active project.
    /// </summary>
    /// <param name="AlternativeContextDocuments">Holds a list of documents that provide additional context for IntelliSense features.</param>
    /// <param name="ActiveProject">Represents the currently active project within the IntelliSense context.</param>
    public IntelliSenseProjectContextContainer(
        IReadOnlyList<Document> AlternativeContextDocuments,
        Project? ActiveProject)
    {
        this.AlternativeContextDocuments = AlternativeContextDocuments;
        this.ActiveProject = ActiveProject;
    }
}