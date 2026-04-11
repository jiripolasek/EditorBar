// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Reflection;
using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Helpers;

internal static class RoslynCompatibilityHelper
{
    private const string ExtensionBlockDeclarationSyntaxName = "ExtensionBlockDeclarationSyntax";
    private const string ExtensionTypeKindName = "Extension";

    internal static bool IsExtensionType(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind.ToString() == ExtensionTypeKindName;
    }

    internal static bool TryGetExtensionBlockKeyword(this SyntaxNode syntaxNode, out SyntaxToken keyword)
    {
        if (syntaxNode.Language != LanguageNames.CSharp ||
            syntaxNode.GetType().Name != ExtensionBlockDeclarationSyntaxName)
        {
            keyword = default;
            return false;
        }

        var keywordProperty = syntaxNode.GetType().GetProperty("Keyword", BindingFlags.Public | BindingFlags.Instance);
        if (keywordProperty?.GetValue(syntaxNode) is not SyntaxToken extensionKeyword ||
            extensionKeyword == default)
        {
            keyword = default;
            return false;
        }

        keyword = extensionKeyword;
        return true;
    }
}
