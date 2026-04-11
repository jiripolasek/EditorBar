// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
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
    /// Initializes a new instance of the <see cref="AnchorPoint"/> class.
    /// </summary>
    /// <param name="filePath">The file path of the anchor point.</param>
    /// <param name="textSpan">The text span of the anchor point.</param>
    public AnchorPoint(string filePath, AnchorPointTextSpan textSpan = default)
    {
        this.FilePath = filePath;
        this.TextSpan = textSpan;
    }

    /// <summary>
    /// Deconstructs the <see cref="AnchorPoint" /> record into its components.
    /// </summary>
    /// <param name="filePath">The file path of the anchor point.</param>
    /// <param name="textSpan">The text span of the anchor point.</param>
    public void Deconstruct(out string filePath, out AnchorPointTextSpan textSpan)
    {
        filePath = this.FilePath;
        textSpan = this.TextSpan;
    }
}
