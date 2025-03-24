// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Threading;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

public class WorkspaceLocationProvider : ILocationProvider
{
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="wpfTextView" /> is <see langword="null" />.</exception>
    public Task<LocationNavModel?> CreateAsync(IWpfTextView wpfTextView, CancellationToken cancellationToken = default)
    {
        Requires.NotNull(wpfTextView, nameof(wpfTextView));

        var textContainer = wpfTextView.TextBuffer!.AsTextContainer();

        return Workspace.TryGetWorkspace(textContainer, out var workspace)
            ? Task.FromResult(CreateFromWorkspace(workspace, textContainer))
            : Task.FromResult<LocationNavModel?>(null);
    }


    private static LocationNavModel? CreateFromWorkspace(Workspace workspace, SourceTextContainer sourceTextContainer)
    {
        Requires.NotNull(workspace, nameof(workspace));

        var documentId = workspace.GetDocumentIdInCurrentContext(sourceTextContainer);

        var document = workspace.CurrentSolution.GetDocument(documentId);
        if (document == null)
        {
            return null;
        }

        var relatedDocumentIds = workspace.GetRelatedDocumentIds(sourceTextContainer).ToList();
        // if there's more that one related document, we probably have multiple target frameworks
        // let's check if all documents have same file path (just to be sure) and different project ids
        if (relatedDocumentIds.Count > 1)
        {
            var filePath = document.FilePath;
            foreach (var relatedDocumentId in relatedDocumentIds)
            {
                var relatedDocument = workspace.CurrentSolution.GetDocument(relatedDocumentId);
                if (relatedDocument?.FilePath == filePath)
                {
                    return null;
                }
            }
        }

        return null;
    }
}