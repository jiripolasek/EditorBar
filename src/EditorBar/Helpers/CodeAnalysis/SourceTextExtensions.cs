// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace JPSoftworks.EditorBar.Helpers;

internal static class SourceTextExtensions
{
    /// <summary>
    /// Gets the documents from the corresponding workspace's current solution that are associated with the text container.
    /// </summary>
    /// <param name="sourceTextContainer">The text container.</param>
    /// <param name="workspace">The workspace.</param>
    public static IntelliSenseProjectContextContainer GetProjectsAssociatedWithDocument(
        this SourceTextContainer sourceTextContainer,
        Workspace workspace)
    {
        Requires.NotNull(sourceTextContainer, nameof(sourceTextContainer));
        Requires.NotNull(workspace, nameof(workspace));

        var relatedDocumentsTemp = sourceTextContainer.GetRelatedDocuments();
        switch (relatedDocumentsTemp.Length)
        {
            case 0:
                return IntelliSenseProjectContextContainer.Empty;
            case 1:
                return new IntelliSenseProjectContextContainer(relatedDocumentsTemp, relatedDocumentsTemp[0].Project);
            case > 1:
                {
                    var relatedDocuments = relatedDocumentsTemp
                        .OrderBy(static projectItem => projectItem?.Name)
                        .ToImmutableArray();

                    var documentInCurrentContext = sourceTextContainer.GetOpenDocumentInCurrentContext();
                    var selectedProjectItem = documentInCurrentContext != null
                        ? relatedDocuments.FirstOrDefault(document =>
                            document.Project.Name == documentInCurrentContext.Project.Name) ?? relatedDocuments.First()
                        : relatedDocuments.First();

                    return new IntelliSenseProjectContextContainer(relatedDocuments, selectedProjectItem?.Project);
                }
            default:
                throw new InvalidOperationException("Invalid case.");
        }
    }

    /// <summary>
    /// Retrieves the project associated with a given document in a specified workspace.
    /// </summary>
    /// <param name="sourceTextContainer">Represents the source text of the document for which the project is being retrieved.</param>
    /// <param name="workspace">Represents the workspace context in which the document exists.</param>
    /// <returns>Returns the project associated with the document, or null if no project is found.</returns>
    public static Project? GetProjectForDocument(this SourceTextContainer sourceTextContainer, Workspace workspace)
    {
        Requires.NotNull(sourceTextContainer, nameof(sourceTextContainer));
        Requires.NotNull(workspace, nameof(workspace));

        var id = workspace.GetDocumentIdInCurrentContext(sourceTextContainer);
        var document = workspace.CurrentSolution.GetDocument(id);
        if (document != null)
        {
            return document.Project;
        }

        var additionalDocument = workspace.CurrentSolution.GetAdditionalDocument(id);
        if (additionalDocument != null)
        {
            return additionalDocument.Project;
        }

        var analyzerConfigDocument = workspace.CurrentSolution.GetAnalyzerConfigDocument(id);
        if (analyzerConfigDocument != null)
        {
            return analyzerConfigDocument.Project;
        }

        return null;
    }

    /// <summary>
    /// Gets the documents from the corresponding workspace's current solution that are associated with the text container.
    /// </summary>
    public static IntelliSenseProjectContextContainer GetProjectsAssociatedWithDocument(
        this SourceTextContainer sourceTextContainer)
    {
        Requires.NotNull(sourceTextContainer, nameof(sourceTextContainer));

        return !Workspace.TryGetWorkspace(sourceTextContainer, out var workspace)
            ? IntelliSenseProjectContextContainer.Empty
            : GetProjectsAssociatedWithDocument(sourceTextContainer, workspace);
    }


    private static Document? GetOpenDocumentInCurrentContext(this SourceTextContainer sourceTextContainer)
    {
        if (!Workspace.TryGetWorkspace(sourceTextContainer, out var workspace))
        {
            return null;
        }

        var id = workspace.GetDocumentIdInCurrentContext(sourceTextContainer);
        return workspace.CurrentSolution.GetDocument(id);
    }

    private static ImmutableArray<Document> GetRelatedDocuments(this SourceTextContainer sourceTextContainer)
    {
        if (!Workspace.TryGetWorkspace(sourceTextContainer, out var workspace))
        {
            return ImmutableArray<Document>.Empty;
        }

        var solution = workspace.CurrentSolution;
        var documentId = workspace.GetDocumentIdInCurrentContext(sourceTextContainer);
        if (documentId == null)
        {
            return ImmutableArray<Document>.Empty;
        }

        var relatedIds = workspace.GetRelatedDocumentIds(sourceTextContainer);
        return relatedIds.Select(solution.GetDocument).Where(static t => t != null).ToImmutableArray()!;
    }
}