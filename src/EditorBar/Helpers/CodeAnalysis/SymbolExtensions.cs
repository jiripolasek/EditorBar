// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Helper methods for working with <see cref="ISymbol" /> instances.
/// </summary>
internal static class SymbolExtensions
{
    /// <summary>
    /// Returns a fully qualified metadata name of the form
    /// Namespace.OuterType+NestedType`2
    /// which can be passed to <c>Compilation.GetTypeByMetadataName</c>.
    /// </summary>
    public static string GetFullMetadataName(this INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol is null)
        {
            throw new ArgumentNullException(nameof(typeSymbol));
        }

        // We build in *reverse* (from the innermost containing type upwards).
        var parts = new Stack<string>();
        var current = typeSymbol;

        // Collect nested type segments (e.g. Outer+Inner`1+InnerInner`2)
        while (current != null)
        {
            parts.Push(current.MetadataName);
            current = current.ContainingType;
        }

        // Now, parts contains:
        //   [NestedClass`1, OuterClass`2, ... up to top-level type]
        // We'll join them with '+' but first we need the namespace.
        // Once we hit a top-level type, we look at the namespace chain.

        var topLevelType = typeSymbol;
        while (topLevelType.ContainingType != null)
        {
            topLevelType = topLevelType.ContainingType;
        }

        // Gather namespaces from inside out, stopping at the global namespace
        var namespaces = new Stack<string>();
        var containingNamespace = topLevelType.ContainingNamespace;
        while (containingNamespace is { IsGlobalNamespace: false })
        {
            namespaces.Push(containingNamespace.MetadataName);
            containingNamespace = containingNamespace.ContainingNamespace;
        }

        // Build the final string:
        //  1) namespace segments joined by '.'
        //  2) plus sign(s) for nested types
        // Example: "MyNamespace.OuterType+NestedType`1"
        var fullName = string.Join(".", namespaces);
        if (fullName.Length > 0)
        {
            fullName += ".";
        }

        // Join all type parts with '+'
        fullName += string.Join("+", parts);

        return fullName;
    }
}