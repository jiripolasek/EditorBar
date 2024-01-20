// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides a set of static methods for working with strings marked with nullability attributes.
/// </summary>
internal static class StringHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] string? input)
    {
        return string.IsNullOrWhiteSpace(input!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] string? input)
    {
        return string.IsNullOrEmpty(input!);
    }
}