// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

public record SymbolAnchorPoint : AnchorPoint
{
    public ISymbol Symbol { get; }
    public Location Location { get; }

    public SymbolAnchorPoint(ISymbol Symbol, Location Location)
        : base(Location.SourceTree!.FilePath,
            new AnchorPointTextSpan(Location.SourceSpan.Start, Location.SourceSpan.Length))
    {
        this.Symbol = Symbol;
        this.Location = Location;
    }
}