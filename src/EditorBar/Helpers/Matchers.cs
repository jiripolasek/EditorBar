// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides extension methods for matching enum flags.
/// </summary>
public static class Matchers
{
    /// <summary>
    /// Determines whether all flags in the specified <paramref name="flags" /> are set in the <paramref name="usage" />.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="usage">The enum value to check.</param>
    /// <param name="flags">The flags to match.</param>
    /// <returns>
    /// <c>true</c> if all flags in <paramref name="flags" /> are set in <paramref name="usage" />; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <typeparamref name="T" /> is not an enum type.</exception>
    public static bool MatchFlags<T>(this T usage, T flags) where T : struct
    {
        // Ensure the type parameter is indeed an enum
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException($"Generic parameter {nameof(T)} must be an enum type.");
        }

        // Convert both usage and flags into their underlying integral values
        var usageValue = Convert.ToUInt64(usage);
        var flagsValue = Convert.ToUInt64(flags);

        // Perform a bitwise AND and check if all bits in flagsValue are also set in usageValue
        return (usageValue & flagsValue) == flagsValue;
    }

    /// <summary>
    /// Determines whether any flag in the specified <paramref name="flags" /> is set in the <paramref name="usage" />.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="usage">The enum value to check.</param>
    /// <param name="flags">The flags to match.</param>
    /// <returns>
    /// <c>true</c> if any flag in <paramref name="flags" /> is set in <paramref name="usage" />; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <typeparamref name="T" /> is not an enum type.</exception>
    public static bool HasAnyFlag<T>(this T usage, T flags) where T : struct
    {
        // Ensure the type parameter is indeed an enum
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException($"Generic parameter {nameof(T)} must be an enum type.");
        }

        // Convert both usage and flags into their underlying integral values
        var usageValue = Convert.ToUInt64(usage);
        var flagsValue = Convert.ToUInt64(flags);

        // Perform a bitwise AND and check if all bits in flagsValue are also set in usageValue
        return (usageValue & flagsValue) != 0;
    }
}