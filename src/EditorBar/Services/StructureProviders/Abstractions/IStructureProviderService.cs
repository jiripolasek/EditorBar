// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

internal interface IStructureProviderService
{
    /// <summary>
    /// Returns the appropriate structure provider for the given text view and workspace.
    /// </summary>
    IStructureProvider CreateProvider(ITextView textView, Workspace? workspace);
}