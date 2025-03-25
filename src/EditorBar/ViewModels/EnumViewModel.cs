// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace JPSoftworks.EditorBar.ViewModels;

/// <summary>
/// Represents a view model for an enumeration.
/// </summary>
/// <typeparam name="TValue">The type of the enumeration.</typeparam>
public class EnumViewModel<TValue>
{
    /// <summary>
    /// Gets or sets the display name of the enumeration value.
    /// </summary>
    [UsedImplicitly]
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the value of the enumeration.
    /// </summary>
    [UsedImplicitly]
    public required TValue? Value { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumViewModel{T}" /> class.
    /// </summary>
    /// <param name="value">The value of the enumeration.</param>
    [SetsRequiredMembers]
    public EnumViewModel(TValue? value)
    {
        this.DisplayName = value == null ? "-" : value.ToString();
        this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumViewModel{T}" /> class.
    /// </summary>
    /// <param name="value">The value of the enumeration.</param>
    /// <param name="displayName">The display name of the enumeration value.</param>
    [SetsRequiredMembers]
    public EnumViewModel(TValue? value, string displayName)
    {
        this.DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        this.Value = value;
    }
}