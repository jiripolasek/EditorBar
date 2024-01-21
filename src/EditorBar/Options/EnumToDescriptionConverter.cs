// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace JPSoftworks.EditorBar.Options;

internal class EnumToDescriptionConverter(Type type) : EnumConverter(type)
{
    private readonly Type _enumType = type;

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
    {
        return destType == typeof(string);
    }

    /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
    /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
    /// <exception cref="ArgumentException">Value is not valid enum member.</exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object? value, Type destType)
    {
        if (value == null)
            throw new ArgumentException("Value is not valid enum member.", nameof(value));

        var fieldName = Enum.GetName(_enumType, value);
        if (fieldName == null)
            throw new ArgumentException("Value is not valid enum member.", nameof(value));

        var fi = _enumType.GetField(fieldName);
        if (fi == null)
            throw new ArgumentException("Value is not valid enum member.", nameof(value));

        var descriptionAttribute = fi.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute != null && !string.IsNullOrEmpty(descriptionAttribute.Description!)
            ? descriptionAttribute.Description!
            : value.ToString();
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
    {
        return srcType == typeof(string);
    }

    /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
    /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
    /// <exception cref="OverflowException"><paramref name="value" /> is outside the range of the underlying type of enum type.</exception>
    /// <exception cref="ArgumentException">Invalid type of the value.</exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
    {
        if (value is not string stringValue)
            throw new ArgumentException("Unsupported type of value.", nameof(value));

        foreach (var fi in _enumType.GetFields())
        {
            var descriptionAttribute = fi.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null && stringValue == descriptionAttribute.Description)
            {
                return Enum.Parse(_enumType, fi.Name);
            }
        }

        return Enum.Parse(_enumType, stringValue);
    }
}