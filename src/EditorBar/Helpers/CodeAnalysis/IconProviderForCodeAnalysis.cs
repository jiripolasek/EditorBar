// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Imaging;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Retrieves an image ID based on the kind and accessibility of a given symbol.
/// </summary>
public static class IconProviderForCodeAnalysis
{
    /// <summary>
    /// Gets the image ID for a symbol based on its kind and accessibility.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static int GetImageId(this ISymbol symbol)
    {
        return symbol.Kind switch
        {
            SymbolKind.Assembly => KnownImageIds.Assembly,
            SymbolKind.DynamicType => KnownImageIds.StatusHelp,
            SymbolKind.Event => GetEventImageId((IEventSymbol)symbol),
            SymbolKind.Field => GetFieldImageId((IFieldSymbol)symbol),
            SymbolKind.Label => KnownImageIds.Label,
            SymbolKind.Local => IconIds.LocalVariable,
            SymbolKind.Method => GetMethodImageId((IMethodSymbol)symbol),
            SymbolKind.NamedType => GetTypeImageId((INamedTypeSymbol)symbol),
            SymbolKind.Namespace => IconIds.Namespace,
            SymbolKind.Parameter => IconIds.Argument,
            SymbolKind.Property => GetPropertyImageId((IPropertySymbol)symbol),
            SymbolKind.Discard => IconIds.Discard,
            _ => KnownImageIds.Item
        };

        static int GetEventImageId(IEventSymbol eventSymbol)
        {
            return eventSymbol.DeclaredAccessibility switch
            {
                Accessibility.Public => KnownImageIds.EventPublic,
                Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.EventProtected,
                Accessibility.Private => eventSymbol.ExplicitInterfaceImplementations.Length != 0
                    ? IconIds.ExplicitInterfaceEvent
                    : KnownImageIds.EventPrivate,
                Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.EventInternal,
                _ => IconIds.Event
            };
        }

        static int GetFieldImageId(IFieldSymbol fieldSymbol)
        {
            if (fieldSymbol.IsConst)
            {
                if (fieldSymbol.ContainingType?.TypeKind == TypeKind.Enum)
                {
                    return IconIds.EnumField;
                }

                return fieldSymbol.DeclaredAccessibility switch
                {
                    Accessibility.Public => KnownImageIds.ConstantPublic,
                    Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.ConstantProtected,
                    Accessibility.Private => KnownImageIds.ConstantPrivate,
                    Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.ConstantInternal,
                    _ => KnownImageIds.Constant
                };
            }

            return fieldSymbol.DeclaredAccessibility switch
            {
                Accessibility.Public => KnownImageIds.FieldPublic,
                Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.FieldProtected,
                Accessibility.Private => KnownImageIds.FieldPrivate,
                Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.FieldInternal,
                _ => IconIds.Field
            };
        }

        static int GetMethodImageId(IMethodSymbol m)
        {
            switch (m.MethodKind)
            {
                case MethodKind.Constructor:
                case MethodKind.StaticConstructor:
                    return m.DeclaredAccessibility switch
                    {
                        Accessibility.Public => IconIds.PublicConstructor,
                        Accessibility.Protected or Accessibility.ProtectedOrInternal => IconIds.ProtectedConstructor,
                        Accessibility.Private => IconIds.PrivateConstructor,
                        Accessibility.ProtectedAndInternal or Accessibility.Internal => IconIds.InternalConstructor,
                        _ => IconIds.Constructor
                    };
                case MethodKind.Destructor:
                    return IconIds.Destructor;
                case MethodKind.UserDefinedOperator:
                case MethodKind.Conversion:
                    return m.DeclaredAccessibility switch
                    {
                        Accessibility.Public => KnownImageIds.OperatorPublic,
                        Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.OperatorProtected,
                        Accessibility.Private => KnownImageIds.OperatorPrivate,
                        Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.OperatorInternal,
                        _ => KnownImageIds.Operator
                    };
            }

            return m.DeclaredAccessibility switch
            {
                Accessibility.Public => KnownImageIds.MethodPublic,
                Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.MethodProtected,
                Accessibility.Private => m.ExplicitInterfaceImplementations.Length != 0
                    ? IconIds.ExplicitInterfaceMethod
                    : KnownImageIds.MethodPrivate,
                Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.MethodInternal,
                _ => IconIds.Method
            };
        }

        static int GetTypeImageId(INamedTypeSymbol t)
        {
            switch (t.TypeKind)
            {
                case TypeKind.Class:
                    return t.DeclaredAccessibility switch
                    {
                        Accessibility.Public => KnownImageIds.ClassPublic,
                        Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.ClassProtected,
                        Accessibility.Private => KnownImageIds.ClassPrivate,
                        Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.ClassInternal,
                        _ => IconIds.Class
                    };
                case TypeKind.Delegate:
                    return t.DeclaredAccessibility switch
                    {
                        Accessibility.Public => KnownImageIds.DelegatePublic,
                        Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.DelegateProtected,
                        Accessibility.Private => KnownImageIds.DelegatePrivate,
                        Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.DelegateInternal,
                        _ => IconIds.Delegate
                    };
                case TypeKind.Enum:
                    return t.DeclaredAccessibility switch
                    {
                        Accessibility.Public => KnownImageIds.EnumerationPublic,
                        Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds
                            .EnumerationProtected,
                        Accessibility.Private => KnownImageIds.EnumerationPrivate,
                        Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds
                            .EnumerationInternal,
                        _ => IconIds.Enum
                    };
                case TypeKind.Interface:
                    switch (t.DeclaredAccessibility)
                    {
                        case Accessibility.Public:
                            {
                                var implementsIDisposable =
                                    t.AllInterfaces.Any(i => i.Name is "IDisposable" or "IAsyncDisposable");
                                if (implementsIDisposable)
                                {
                                    return IconIds.Disposable;
                                }

                                return KnownImageIds.InterfacePublic;
                            }
                        case Accessibility.Protected:
                        case Accessibility.ProtectedOrInternal:
                            return KnownImageIds.InterfaceProtected;
                        case Accessibility.Private: return KnownImageIds.InterfacePrivate;
                        case Accessibility.ProtectedAndInternal:
                        case Accessibility.Internal: return KnownImageIds.InterfaceInternal;
                        default: return IconIds.Interface;
                    }
                case TypeKind.Struct:
                    return t.DeclaredAccessibility switch
                    {
                        Accessibility.Public => KnownImageIds.ValueTypePublic,
                        Accessibility.Protected or Accessibility.ProtectedOrInternal =>
                            KnownImageIds.ValueTypeProtected,
                        Accessibility.Private => KnownImageIds.ValueTypePrivate,
                        Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.ValueTypeInternal,
                        _ => IconIds.ValueType
                    };
                case TypeKind.Module:
                    return t.DeclaredAccessibility switch
                    {
                        Accessibility.Public => KnownImageIds.ModulePublic,
                        Accessibility.Protected or Accessibility.ProtectedOrInternal => KnownImageIds.ModuleProtected,
                        Accessibility.Private => KnownImageIds.ModulePrivate,
                        Accessibility.ProtectedAndInternal or Accessibility.Internal => KnownImageIds.ModuleInternal,
                        _ => KnownImageIds.Module
                    };

                case TypeKind.TypeParameter:
                    return KnownImageIds.Type;
                default:
                    return KnownImageIds.StatusError;
            }
        }

        static int GetPropertyImageId(IPropertySymbol p)
        {
            switch (p.DeclaredAccessibility)
            {
                case Accessibility.Public: return KnownImageIds.PropertyPublic;
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                    return KnownImageIds.PropertyProtected;
                case Accessibility.Private:
                    return p.ExplicitInterfaceImplementations.Length != 0
                        ? IconIds.ExplicitInterfaceProperty
                        : KnownImageIds.PropertyPrivate;
                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal: return KnownImageIds.PropertyInternal;
                default: return KnownImageIds.Property;
            }
        }
    }
}