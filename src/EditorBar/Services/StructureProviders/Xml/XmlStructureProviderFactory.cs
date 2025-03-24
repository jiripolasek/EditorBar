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

namespace JPSoftworks.EditorBar.Services.StructureProviders.Xml;

[Export(typeof(IStructureProviderFactory))]
[Name(nameof(XmlStructureProviderFactory))]
[Order(Before = nameof(PlainTextStructureProviderFactory))]
internal class XmlStructureProviderFactory : IStructureProviderFactory
{
    public bool CanHandle(ITextView textView, Workspace? workspace)
    {
        return textView.TextBuffer?.ContentType?.IsOfType("xml") == true;
    }

    public IStructureProvider Create(ITextView textView)
    {
        return new XmlFileStructureProvider(textView);
    }
}