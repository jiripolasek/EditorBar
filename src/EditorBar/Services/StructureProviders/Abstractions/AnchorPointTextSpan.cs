// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Represents a span of text with a starting position and a specified length.
/// </summary>
public readonly record struct AnchorPointTextSpan
{
    /// <summary>
    /// A text span that represents an empty span.
    /// </summary>
    public static readonly AnchorPointTextSpan Zero = new();

    /// <summary>
    /// Gets the starting position of the text span.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the number of characters in the text span.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the end position of the text span.
    /// </summary>
    public int End => this.Start + this.Length;

    /// <summary>
    /// Gets a value indicating whether the text span is empty.
    /// </summary>
    public bool IsEmpty => this.Length == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnchorPointTextSpan" /> struct.
    /// </summary>
    /// <param name="start">The starting position of the text span.</param>
    /// <param name="length">The number of characters in the text span.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="start" /> or <paramref name="length" /> is negative.
    /// </exception>
    public AnchorPointTextSpan(int start, int length)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start), start, "Start must be non-negative.");
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be non-negative.");

        this.Start = start;
        this.Length = length;
    }

    /// <summary>
    /// Determines whether the text span contains the specified position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns><c>true</c> if the position is within the span; otherwise, <c>false</c>.</returns>
    public bool Contains(int position) => position >= this.Start && position < this.End;


    /// <summary>
    /// Returns a string that represents the current text span.
    /// </summary>
    /// <returns>A string in the format [Start..End).</returns>
    public override string ToString() => $"[{this.Start}..{this.End})";


    /// <summary>
    /// Deconstructs the text span into its start and length components.
    /// </summary>
    /// <param name="start">The starting position.</param>
    /// <param name="length">The length.</param>
    public void Deconstruct(out int start, out int length)
    {
        start = this.Start;
        length = this.Length;
    }
}