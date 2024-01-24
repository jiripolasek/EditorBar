// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace JPSoftworks.EditorBar.Presentation;

/// <summary>
/// Represents a converter that converts a boolean value to a specified type.
/// </summary>
/// <typeparam name="T">The type to convert the boolean value to.</typeparam>
public class BooleanConverter<T>(T trueValue, T falseValue) : IValueConverter
{
    /// <summary>
    /// Gets or sets the value to be returned when the input value is true.
    /// </summary>
    [PublicAPI]
    public T True { get; set; } = trueValue;

    /// <summary>
    /// Gets or sets the value to be returned when the input value is false.
    /// </summary>
    [PublicAPI]
    public T False { get; set; } = falseValue;

    /// <summary>
    /// Converts a boolean value to the specified type.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type to convert the boolean value to.</param>
    /// <param name="parameter">An optional parameter.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The converted value.</returns>
    public virtual object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? this.True : this.False;
    }

    /// <summary>
    /// Converts a value back to a boolean value.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The type to convert the value back to.</param>
    /// <param name="parameter">An optional parameter.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The converted boolean value.</returns>
    public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is T t && EqualityComparer<T>.Default.Equals(t, this.True);
    }
}
