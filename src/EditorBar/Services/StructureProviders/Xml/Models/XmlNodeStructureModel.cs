// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.VisualStudio.Imaging;

namespace JPSoftworks.EditorBar.Services.StructureProviders.Xml.Models;

/// <summary>
/// Provides structural node model for XML document node.
/// </summary>
public class XmlNodeStructureModel : BaseStructureModel, IEquatable<XmlNodeStructureModel>
{
    /// <summary>
    /// Path from this the document root to this segment.
    /// </summary>
    public XPathSegment[]? Path { get; }

    /// <summary>
    /// Initializes an instance of XmlNodeStructureModel with a file path, display name, and a span of XPath segments.
    /// </summary>
    /// <param name="filePath">Specifies the location of the XML file to be processed.</param>
    /// <param name="displayName">Provides a user-friendly name for the XML structure.</param>
    /// <param name="path">Defines the sequence of XPath segments for navigating the XML structure.</param>
    public XmlNodeStructureModel(string filePath, string displayName, Span<XPathSegment> path)
        : base(displayName, KnownMonikers.MarkupXML, new AnchorPoint(filePath))
    {
        this.Path = path.ToArray();
    }

    /// <inheritdoc />
    public bool Equals(XmlNodeStructureModel? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && Equals(this.Path!, other.Path!);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((XmlNodeStructureModel)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), this.Path);
    }

    /// <summary>
    /// Compares two instances of <see cref="XmlNodeStructureModel" /> for equality.
    /// </summary>
    /// <param name="left">The first instance to compare for equality.</param>
    /// <param name="right">The second instance to compare for equality.</param>
    /// <returns>True if both instances are equal; otherwise, false.</returns>
    public static bool operator ==(XmlNodeStructureModel? left, XmlNodeStructureModel? right)
    {
        return Equals(left!, right!);
    }

    /// <summary>
    /// Compares two <see cref="XmlNodeStructureModel" /> instances for inequality. Returns true if they are not equal, false otherwise.
    /// </summary>
    /// <param name="left">The first instance to compare for inequality.</param>
    /// <param name="right">The second instance to compare for inequality.</param>
    /// <returns>A boolean indicating whether the two instances are not equal.</returns>
    public static bool operator !=(XmlNodeStructureModel? left, XmlNodeStructureModel? right)
    {
        return !Equals(left!, right!);
    }
}