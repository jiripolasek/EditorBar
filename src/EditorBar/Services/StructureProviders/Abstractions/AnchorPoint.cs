// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Represents a point in the text document.
/// </summary>
public record AnchorPoint
{
    /// <summary>
    /// Gets the file path of the anchor point.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the text span of the anchor point.
    /// </summary>
    public AnchorPointTextSpan TextSpan { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnchorPoint" /> record.
    /// </summary>
    /// <param name="FilePath">The file path of the anchor point.</param>
    /// <param name="TextSpan">The text span of the anchor point.</param>
    public AnchorPoint(string FilePath, AnchorPointTextSpan TextSpan = default)
    {
        this.FilePath = FilePath;
        this.TextSpan = TextSpan;
    }

    /// <summary>
    /// Deconstructs the <see cref="AnchorPoint" /> record into its components.
    /// </summary>
    /// <param name="FilePath">The file path of the anchor point.</param>
    /// <param name="TextSpan">The text span of the anchor point.</param>
    public void Deconstruct(out string FilePath, out AnchorPointTextSpan TextSpan)
    {
        FilePath = this.FilePath;
        TextSpan = this.TextSpan;
    }
}