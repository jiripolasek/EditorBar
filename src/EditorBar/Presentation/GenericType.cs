// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows.Markup;

namespace JPSoftworks.EditorBar.Presentation;

internal class GenericType : MarkupExtension
{
    public Type? BaseType { get; set; }

    public Type? InnerType { get; set; }

    public Type[]? InnerTypes { get; set; }

    public GenericType() { }

    public GenericType(Type baseType, Type innerType)
    {
        this.BaseType = baseType;
        this.InnerType = innerType;
    }

    public GenericType(Type baseType, params Type[] innerTypes)
    {
        this.BaseType = baseType;
        this.InnerTypes = innerTypes;
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (this.BaseType == null)
        {
            return null;
        }

        if (this.InnerType != null)
        {
            return this.BaseType.MakeGenericType(this.InnerType);
        }

        if (this.InnerTypes != null && this.InnerTypes.Length != 0)
        {
            return this.BaseType.MakeGenericType(this.InnerTypes);
        }

        return this.BaseType;
    }
}