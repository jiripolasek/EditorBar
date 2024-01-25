// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

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
    public string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the value of the enumeration.
    /// </summary>
    public TValue? Value { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumViewModel{T}"/> class.
    /// </summary>
    /// <param name="value">The value of the enumeration.</param>
    public EnumViewModel(TValue? value)
    {
        this.DisplayName = value == null ? "-" : value.ToString();
        this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumViewModel{T}"/> class.
    /// </summary>
    /// <param name="value">The value of the enumeration.</param>
    /// <param name="displayName">The display name of the enumeration value.</param>
    public EnumViewModel(TValue? value, string displayName)
    {
        this.DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        this.Value = value;
    }
}
