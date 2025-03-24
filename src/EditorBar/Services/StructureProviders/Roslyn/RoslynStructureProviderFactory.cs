// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Services.StructureProviders.PlainText;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JPSoftworks.EditorBar.Services.StructureProviders.Roslyn;

[Export(typeof(IStructureProviderFactory))]
[Name(nameof(RoslynStructureProviderFactory))]
[Order(Before = nameof(PlainTextFileStructureProvider))]
internal class RoslynStructureProviderFactory : IStructureProviderFactory
{
    public bool CanHandle(ITextView textView, Workspace? workspace)
    {
        return workspace != null;
    }

    public IStructureProvider Create(ITextView textView)
    {
        return new RoslynObservableStructureProvider(textView);
    }
}