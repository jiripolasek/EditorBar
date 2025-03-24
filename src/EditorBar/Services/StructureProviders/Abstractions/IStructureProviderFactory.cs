// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

internal interface IStructureProviderFactory
{
    /// <summary>
    /// Checks if the factory can create a provider for the given context.
    /// </summary>
    bool CanHandle(ITextView textView, Workspace? workspace);

    /// <summary>
    /// Creates an instance of the file structure provider.
    /// </summary>
    IStructureProvider Create(ITextView textView);
}