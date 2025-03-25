// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reflection;
using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Helpers;

internal static class WorkspaceExtensions
{
    public static void SetDocumentContextHack(this Workspace workspace, DocumentId document)
    {
        try
        {
            // Use reflection to call the internal method `void SetDocumentContext(DocumentId documentId)` and
            // property `bool CanChangeActiveContextDocument` on Workspace

            // Can we change context?
            var propertyInfo = workspace.GetType().GetProperty("CanChangeActiveContextDocument",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException(
                    "Property CanChangeActiveContextDocument not found on Workspace type.");
            }

            var canChangeContextResult = (bool)propertyInfo.GetValue(workspace);

            if (canChangeContextResult)
            {
                var method = workspace.GetType()
                    .GetMethod("SetDocumentContext", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                {
                    throw new MissingMethodException("Method SetDocumentContext not found on Workspace type.");
                }

                method.Invoke(workspace, [document]);
            }
        }
        catch (Exception ex)
        {
            ex.Log(
                "Failed to change document context. This is ugly hack using reflection to access internal mechanics.");
        }
    }
}