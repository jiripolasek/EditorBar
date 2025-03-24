// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JPSoftworks.EditorBar.Services.StructureProviders.PlainText;

[Export(typeof(IStructureProviderFactory))]
[Name(nameof(PlainTextFileStructureProvider))]
internal class PlainTextStructureProviderFactory : IStructureProviderFactory
{
    public bool CanHandle(ITextView textView, Workspace? workspace)
    {
        return true;
    }

    public IStructureProvider Create(ITextView textView)
    {
        return new PlainTextFileStructureProvider(textView);
    }
}