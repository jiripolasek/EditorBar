// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Xml.Linq;

namespace JPSoftworks.EditorBar.Services.StructureProviders.Xml;

public readonly record struct XPathSegment(
    XName Name,
    int Index,
    string? Prefix,
    XPathSegmentKind Kind = XPathSegmentKind.Element)
{
    public bool CanHaveChildren { get; init; }

    public override string ToString()
    {
        var nameString = string.IsNullOrWhiteSpace(this.Prefix!)
            ? this.Name.LocalName
            : $"{this.Prefix}:{this.Name.LocalName}";
        return this.Index > 0 ? $"{nameString}[{this.Index + 1}]" : nameString;
    }

    public string ToDisplayString()
    {
        var nameString = string.IsNullOrWhiteSpace(this.Prefix!)
            ? this.Name.LocalName
            : $"{this.Prefix}:{this.Name.LocalName}";
        return this.Index > 0 ? $"{nameString}[{this.Index + 1}]" : nameString;
    }
}