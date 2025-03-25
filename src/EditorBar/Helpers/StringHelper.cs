// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides a set of static methods for working with strings marked with nullability attributes.
/// </summary>
internal static class StringHelper
{
    /// <summary>
    /// Determines whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="input">The string to test.</param>
    /// <returns>true if the input string is null, empty, or consists only of white-space characters; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] string? input)
    {
        return string.IsNullOrWhiteSpace(input!);
    }

    /// <summary>
    /// Determines whether the specified string is null or an empty string.
    /// </summary>
    /// <param name="input">The string to test.</param>
    /// <returns>true if the input string is null or an empty string; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] string? input)
    {
        return string.IsNullOrEmpty(input!);
    }

    /// <summary>
    /// Truncates the string to the specified maximum length and appends an ellipsis ("...") if necessary.
    /// </summary>
    /// <param name="input">The string to truncate.</param>
    /// <param name="maxLength">The maximum length of the truncated string.</param>
    /// <returns>
    /// The truncated string with an ellipsis appended if it exceeds the maximum length; otherwise, the original
    /// string.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CropSafe(this string input, int maxLength)
    {
        return maxLength >= input.Length
            ? input
            : input.Substring(0, maxLength) + "...";
    }
}