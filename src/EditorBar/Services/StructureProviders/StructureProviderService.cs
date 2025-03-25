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

namespace JPSoftworks.EditorBar.Services.StructureProviders;

[Export(typeof(IStructureProviderService))]
internal class StructureProviderService : IStructureProviderService
{
    private readonly PlainTextStructureProviderFactory _fallbackFactory = new();
    private readonly IEnumerable<Lazy<IStructureProviderFactory, IOrderable>> _providerFactories;

    [ImportingConstructor]
    public StructureProviderService(
        [ImportMany] IEnumerable<Lazy<IStructureProviderFactory, IOrderable>> providerFactories)
    {
        this._providerFactories = Orderer.Order(providerFactories)!.ToList();
    }

    public IStructureProvider CreateProvider(ITextView textView, Workspace? workspace)
    {
        var factoryWrapper = this._providerFactories.FirstOrDefault(factory
            => factory.Value?.CanHandle(textView, workspace) == true);
        var factory = factoryWrapper != null ? factoryWrapper.Value! : this._fallbackFactory;
        return factory.Create(textView);
    }
}