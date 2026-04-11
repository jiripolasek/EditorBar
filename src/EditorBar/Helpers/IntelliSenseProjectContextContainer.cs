// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Represents a container for IntelliSense project context, holding alternative documents and an
/// active project.
/// </summary>
public record struct IntelliSenseProjectContextContainer
{
    /// <summary>
    /// Represents an empty IntelliSense project context container.
    /// </summary>
    public static readonly IntelliSenseProjectContextContainer Empty = new([], null);

    /// <summary>
    /// Gets the documents that provide additional context for IntelliSense features.
    /// </summary>
    public IReadOnlyList<Document> AlternativeContextDocuments { get; }

    /// <summary>
    /// Gets the currently active project within the IntelliSense context.
    /// </summary>
    public Project? ActiveProject { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntelliSenseProjectContextContainer"/> struct.
    /// </summary>
    /// <param name="alternativeContextDocuments">The documents that provide additional context
    /// for IntelliSense features.</param>
    /// <param name="activeProject">The currently active project within the IntelliSense context.</param>
    public IntelliSenseProjectContextContainer(
        IReadOnlyList<Document> alternativeContextDocuments,
        Project? activeProject)
    {
        this.AlternativeContextDocuments = alternativeContextDocuments;
        this.ActiveProject = activeProject;
    }
}
