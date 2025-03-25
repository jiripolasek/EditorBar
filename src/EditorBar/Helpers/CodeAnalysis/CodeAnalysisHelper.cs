// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;
using Microsoft;
using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Helpers;

internal static class CodeAnalysisHelper
{
    public static bool IsAwaitable(this ITypeSymbol? type)
    {
        if (type is null || type.IsStatic)
        {
            return false;
        }

        var methodSymbols =
            from item in type.GetMembers(WellKnownMemberNames.GetAwaiter)
            where item.Kind == SymbolKind.Method && !item.IsStatic
            select (IMethodSymbol)item;

        return methodSymbols.Any(static m => m.Parameters.Length == 0 && m.ReturnType.IsAwaiter());
    }

    private static bool IsAwaiter(this ITypeSymbol type)
    {
        return (type.TypeKind != TypeKind.Dynamic
                && type.MatchNamespaces("System", "Runtime", "CompilerServices")
                && type.Name is nameof(TaskAwaiter)
                    or nameof(ValueTaskAwaiter<int>))
               || IsCustomAwaiter(type);
    }

    private static bool MatchNamespaces(this ITypeSymbol typeSymbol, params string[] namespaceSegments)
    {
        Requires.NotNull(typeSymbol, nameof(typeSymbol));
        Requires.NotNull(namespaceSegments, nameof(namespaceSegments));

        var ns = typeSymbol.ContainingNamespace;
        if (ns == null && namespaceSegments.Length == 0)
        {
            return true;
        }

        for (var i = namespaceSegments.Length - 1; i >= 0; i--)
        {
            var item = namespaceSegments[i];
            if (ns == null || ns.IsExplicitNamespace() == false || ns.Name != item)
            {
                return false;
            }

            ns = ns.ContainingNamespace;
        }

        return ns != null && ns.IsExplicitNamespace() == false;
    }

    private static bool IsExplicitNamespace(this INamespaceSymbol ns)
    {
        return ns.IsGlobalNamespace == false;
    }

    private static bool IsCustomAwaiter(ITypeSymbol type)
    {
        var f = 0;
        const int HAS_GET_RESULT = 1, HAS_ON_COMPLETED = 2, HAS_IS_COMPLETED = 4, IS_AWAITER = 7;
        foreach (var item in type.GetMembers())
        {
            if (item.IsStatic)
            {
                continue;
            }

            switch (item.Kind)
            {
                case SymbolKind.Method:
                    var m = (IMethodSymbol)item;
                    if (m.IsGenericMethod)
                    {
                        continue;
                    }

                    switch (m.Name)
                    {
                        case "GetResult":
                            if (m.Parameters.Length == 0)
                            {
                                f |= HAS_GET_RESULT;
                            }

                            continue;
                        case "OnCompleted":
                            var mp = m.Parameters;
                            if (m.ReturnsVoid
                                && mp.Length == 1
                                && mp[0].Type is INamedTypeSymbol pt
                                && pt.IsGenericType == false
                                && pt.MatchTypeName("Action", "System"))
                            {
                                f |= HAS_ON_COMPLETED;
                            }

                            continue;
                    }

                    continue;
                case SymbolKind.Property:
                    if (item.GetReturnType()?.SpecialType == SpecialType.System_Boolean
                        && item.Name == "IsCompleted")
                    {
                        f |= HAS_IS_COMPLETED;
                    }

                    continue;
            }
        }

        return f == IS_AWAITER;
    }

    public static bool MatchTypeName(this ITypeSymbol typeSymbol, string className, params string[] namespaces)
    {
        return typeSymbol.Name == className && MatchNamespaces(typeSymbol, namespaces);
    }

    public static ITypeSymbol GetReturnType(this ISymbol symbol)
    {
        switch (symbol.Kind)
        {
            case SymbolKind.Field: return ((IFieldSymbol)symbol).Type;
            case SymbolKind.Local: return ((ILocalSymbol)symbol).Type;
            case SymbolKind.Method:
                var m = (IMethodSymbol)symbol;
                return m.MethodKind == MethodKind.Constructor ? m.ContainingType : m.ReturnType;
            case SymbolKind.Parameter: return ((IParameterSymbol)symbol).Type;
            case SymbolKind.Property: return ((IPropertySymbol)symbol).Type;
            case SymbolKind.Alias: return GetReturnType(((IAliasSymbol)symbol).Target);
            case SymbolKind.NamedType:
                return (symbol = symbol.AsMethod()) != null
                    ? ((IMethodSymbol)symbol).ReturnType
                    : null;
        }

        return null;
    }

    public static IMethodSymbol AsMethod(this ISymbol symbol)
    {
        switch (symbol.Kind)
        {
            case SymbolKind.Method: return (IMethodSymbol)symbol;
            case SymbolKind.Event: return ((IEventSymbol)symbol).RaiseMethod;
            case SymbolKind.NamedType:
                var t = (INamedTypeSymbol)symbol;
                return t.TypeKind == TypeKind.Delegate ? t.DelegateInvokeMethod : null;
            default: return null;
        }
    }
}