// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Threading;
using JPSoftworks.EditorBar.Resources;
using JPSoftworks.EditorBar.Services.StructureProviders;
using JPSoftworks.EditorBar.Services.StructureProviders.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Editor;
using CSharpDocumentationCommentTriviaSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax;
using CSharpSyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using VisualBasicDocumentationCommentTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax;
using VisualBasicFieldDeclarationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax;
using VisualBasicSyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;

namespace JPSoftworks.EditorBar.Helpers;

internal static class FileStructureHelper
{
    internal const string TopLevelStatementMainMethodName = "<Main>$";

    /// <summary>
    /// Finds the nearest semantic symbol under (or near) the given caret position,
    /// walking up ancestor nodes if necessary. Treats comments and same-line trailing trivia as
    /// declaration context, extends single-name field declarations to their full span, and promotes
    /// positional record parameters to their properties.
    /// </summary>
    /// <param name="semanticModel">A fresh SemanticModel for the current Document.</param>
    /// <param name="root">SyntaxRoot of the Document.</param>
    /// <param name="sourceText">Source text of the Document.</param>
    /// <param name="position">Caret position in the source text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The nearest symbol if found, otherwise null.</returns>
    private static ImmutableArray<SymbolAnchorPoint> FindDeclarationsUnderPosition(
        SemanticModel semanticModel,
        SyntaxNode root,
        SourceText sourceText,
        int position,
        CancellationToken cancellationToken)
    {
        var declarations = new List<SymbolAnchorPoint>();

        var isDeclarationComment =
            TryGetDeclarationCommentOwnerToken(root, position, out var token);
        if (!isDeclarationComment)
        {
            position = GetDeclarationLookupPosition(root, sourceText, position);
            token = root.FindToken(position, true);
        }

        // 1) Find the token at (or just before) the specified position.
        if (token == default)
        {
            return ImmutableArray<SymbolAnchorPoint>.Empty; // empty list
        }

        // Enum separators fall outside member spans
        if (TryGetEnumMemberAroundSeparator(token, sourceText, position, out var enumMember))
        {
            position = enumMember.Span.End - 1;
            token = enumMember.GetLastToken();
        }

        // 2) Walk upward through the syntax node ancestors, looking for declared symbols
        var commentOwnerResolved = false;
        foreach (var node in token.Parent?.AncestorsAndSelf() ?? [])
        {
            var containsPosition = node.Span.Contains(position);
            if (!containsPosition && (!isDeclarationComment || commentOwnerResolved))
            {
                continue;
            }

            var declarationNode = TryGetSoleFieldSymbolDeclaration(node, out var fieldDeclaration)
                ? fieldDeclaration
                : node;

            // Try to get a declared symbol (method, property, class, etc.)
            var declaredSymbol = semanticModel.GetDeclaredSymbol(declarationNode, cancellationToken);
            var declarationLocation = declarationNode.GetLocation();

            if (declaredSymbol is IParameterSymbol parameterSymbol &&
                node is ParameterSyntax parameterSyntax &&
                GetAssociatedSynthesizedRecordProperty(parameterSymbol, parameterSyntax) is { } propertySymbol)
            {
                declaredSymbol = propertySymbol;
                declarationLocation = parameterSyntax.Identifier.GetLocation();
            }

            if (declaredSymbol is not null)
            {
                if (!containsPosition)
                {
                    commentOwnerResolved = true;
                }

                if (declaredSymbol is IMethodSymbol
                    {
                        MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet,
                        AssociatedSymbol: IPropertySymbol
                    })
                {
                    // ignore, we want the property symbol instead, and since we're walking up the tree, we'll find it later
                    // declarations.Add(propertySymbol);
                }
                else
                {
                    if (declarations.Count > 0 &&
                        SymbolEqualityComparer.Default.Equals(declarations.Last().Symbol, declaredSymbol))
                    {
                        continue;
                    }

                    declarations.Add(new SymbolAnchorPoint(declaredSymbol, declarationLocation));
                }
            }
        }

        return declarations.ToImmutableArray();
    }

    private static bool TryGetEnumMemberAroundSeparator(
        SyntaxToken token,
        SourceText sourceText,
        int position,
        out EnumMemberDeclarationSyntax enumMember)
    {
        // The direct-parent check excludes commas inside member initializers
        if (token is
            {
                RawKind: (int)CSharpSyntaxKind.CommaToken,
                Parent: EnumDeclarationSyntax enumDeclaration
            })
        {
            var memberIndex = 0;
            foreach (var separator in enumDeclaration.Members.GetSeparators())
            {
                if (separator == token && memberIndex < enumDeclaration.Members.Count)
                {
                    // The comma belongs to the preceding member
                    enumMember = enumDeclaration.Members[memberIndex];

                    // Same-line context after it belongs to the next member
                    if (position >= separator.Span.End &&
                        memberIndex + 1 < enumDeclaration.Members.Count)
                    {
                        var nextMember = enumDeclaration.Members[memberIndex + 1];
                        var separatorLine = sourceText.Lines.GetLineFromPosition(separator.SpanStart).LineNumber;
                        var nextMemberLine = sourceText.Lines.GetLineFromPosition(nextMember.SpanStart).LineNumber;
                        if (separatorLine == nextMemberLine && position <= nextMember.SpanStart)
                        {
                            enumMember = nextMember;
                        }
                    }

                    return enumMember.Span.Length > 0;
                }

                memberIndex++;
            }
        }

        enumMember = null!;
        return false;
    }

    private static bool TryGetDeclarationCommentOwnerToken(
        SyntaxNode root,
        int position,
        out SyntaxToken ownerToken)
    {
        ownerToken = root.FindToken(position, false);
        if (TokenOwnsDeclarationCommentAtPosition(ownerToken, position))
        {
            return true;
        }

        if (position > 0)
        {
            var precedingToken = root.FindToken(position - 1, false);
            if (TokenOwnsDeclarationCommentAtPosition(precedingToken, position))
            {
                ownerToken = precedingToken;
                return true;
            }
        }

        return false;
    }

    private static bool TokenOwnsDeclarationCommentAtPosition(SyntaxToken token, int position)
    {
        return TriviaListContainsCommentAtPosition(token.LeadingTrivia, position) ||
               TriviaListContainsCommentAtPosition(token.TrailingTrivia, position);
    }

    private static bool TriviaListContainsCommentAtPosition(SyntaxTriviaList triviaList, int position)
    {
        foreach (var trivia in triviaList)
        {
            if ((!trivia.FullSpan.Contains(position) &&
                 (position <= 0 || !trivia.FullSpan.Contains(position - 1))) ||
                !IsCommentTrivia(trivia))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private static bool IsCommentTrivia(SyntaxTrivia trivia)
    {
        if (trivia.HasStructure &&
            trivia.GetStructure() is CSharpDocumentationCommentTriviaSyntax or VisualBasicDocumentationCommentTriviaSyntax)
        {
            return true;
        }

        return trivia.Token.Language switch
        {
            LanguageNames.CSharp => trivia.RawKind is
                (int)CSharpSyntaxKind.SingleLineCommentTrivia or
                (int)CSharpSyntaxKind.MultiLineCommentTrivia,
            LanguageNames.VisualBasic => trivia.RawKind == (int)VisualBasicSyntaxKind.CommentTrivia,
            _ => false
        };
    }

    private static int GetDeclarationLookupPosition(SyntaxNode root, SourceText sourceText, int position)
    {
        if (position <= 0)
        {
            return position;
        }

        var line = sourceText.Lines.GetLineFromPosition(position);

        // Only look backward when the caret is in trailing whitespace or at the end of the line.
        // This prevents a caret on a blank following line from inheriting the previous declaration.
        for (var index = position; index < line.End; index++)
        {
            if (!char.IsWhiteSpace(sourceText[index]))
            {
                return position;
            }
        }

        var probePosition = Math.Min(position - 1, line.End - 1);
        if (probePosition < line.Start)
        {
            return position;
        }

        var precedingToken = root.FindToken(probePosition, true);
        if (precedingToken == default || precedingToken.Span.Length == 0)
        {
            return position;
        }

        var tokenPosition = precedingToken.Span.End - 1;
        return tokenPosition >= line.Start && tokenPosition < line.End
            ? tokenPosition
            : position;
    }

    private static bool TryGetSoleFieldSymbolDeclaration(
        SyntaxNode node,
        out SyntaxNode declaration)
    {
        if (node is BaseFieldDeclarationSyntax csharpFieldDeclaration &&
            csharpFieldDeclaration.Declaration.Variables.Count == 1)
        {
            declaration = csharpFieldDeclaration.Declaration.Variables[0];
            return true;
        }

        if (node is VisualBasicFieldDeclarationSyntax visualBasicFieldDeclaration)
        {
            SyntaxNode? soleName = null;
            foreach (var declarator in visualBasicFieldDeclaration.Declarators)
            {
                foreach (var name in declarator.Names)
                {
                    if (soleName is not null)
                    {
                        declaration = null!;
                        return false;
                    }

                    soleName = name;
                }
            }

            if (soleName is not null)
            {
                declaration = soleName;
                return true;
            }
        }

        declaration = null!;
        return false;
    }

    private static IPropertySymbol? GetAssociatedSynthesizedRecordProperty(
        IParameterSymbol parameterSymbol,
        ParameterSyntax parameterSyntax)
    {
        if (parameterSyntax.Parent?.Parent is not RecordDeclarationSyntax ||
            parameterSymbol.ContainingSymbol is not IMethodSymbol { MethodKind: MethodKind.Constructor } ||
            parameterSymbol.ContainingType is not { IsRecord: true } recordType)
        {
            return null;
        }

        foreach (var member in recordType.GetMembers(parameterSymbol.Name))
        {
            if (member is not IPropertySymbol propertySymbol)
            {
                continue;
            }

            foreach (var syntaxReference in propertySymbol.DeclaringSyntaxReferences)
            {
                if (syntaxReference.SyntaxTree == parameterSyntax.SyntaxTree &&
                    syntaxReference.Span == parameterSyntax.Span)
                {
                    return propertySymbol;
                }
            }
        }

        return null;
    }

    public static async Task<IList<INamedTypeSymbol>> GetAllTypesAsync(Workspace workspace, DocumentId documentId)
    {
        var document = workspace.CurrentSolution.GetDocument(documentId);
        if (document == null)
        {
            return [];
        }

        var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
        if (semanticModel == null)
        {
            return [];
        }

        // get all types in the document (including nested types)
        var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);
        var childs = false ? root.DescendantNodes() : GetNonTypeTypeContainers(root);
        var types = childs.Where(static t => t is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax);
        var symbols = types.Select(t => semanticModel.GetDeclaredSymbol(t)).OfType<INamedTypeSymbol>().ToList();

        return symbols;
    }

    private static IEnumerable<SyntaxNode> GetNonTypeTypeContainers(SyntaxNode? root)
    {
        if (root == null)
        {
            return [];
        }

        // return descendants that are not types, but can contain types - root + namespaces
        SyntaxNode[] roots = [root, .. root.DescendantNodes().Where(static t => t is BaseNamespaceDeclarationSyntax)];
        return roots.SelectMany(static t => t.ChildNodes().Where(static t => t is not BaseNamespaceDeclarationSyntax));
    }

    public static async Task<INamedTypeSymbol?> GetSymbolByNameAsync(
        Workspace workspace,
        DocumentId documentId,
        string metadataName)
    {
        var document = workspace.CurrentSolution.GetDocument(documentId);
        if (document == null)
        {
            return null;
        }

        var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
        var symbol = semanticModel?.Compilation.GetTypeByMetadataName(metadataName);
        return symbol;
    }

    public static Task<ImmutableList<BaseStructureModel>> GetStructureBreadcrumbsAsync(
        CaretPosition caretPosition,
        Workspace workspace,
        DocumentId documentId,
        CancellationToken cancellationToken)
    {
        // Re-fetch the Document from the CURRENT solution
        var document = workspace.CurrentSolution.GetDocument(documentId);
        return document != null
            ? GetStructureBreadcrumbsAsync(caretPosition, document, cancellationToken)
            : Task.FromResult(ImmutableList<BaseStructureModel>.Empty);
    }

    public static async Task<ImmutableList<BaseStructureModel>> GetStructureBreadcrumbsAsync(
        CaretPosition caretPosition,
        Document document,
        CancellationToken cancellationToken)
    {
        // Get the current caret position in the text buffer
        var position = caretPosition.BufferPosition.Position;

        // Convert that to Roslyn’s `SourceText`
        var sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

        // Make sure caretPosition is valid
        if (position > sourceText.Length)
        {
            position = sourceText.Length;
        }

        // Get syntax root + semantic model
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        if (syntaxRoot == null || semanticModel == null)
        {
            return ImmutableList<BaseStructureModel>.Empty;
        }

        // Determine which symbol we’re on
        var declarationsAncherPoints =
            FindDeclarationsUnderPosition(semanticModel, syntaxRoot, sourceText, position, cancellationToken);
        if (declarationsAncherPoints.Length == 0)
        {
            return ImmutableList<BaseStructureModel>.Empty;
        }

        // Manual reverse iteration for better performance
        var result = new List<BaseStructureModel>(declarationsAncherPoints.Length);
        for (var i = declarationsAncherPoints.Length - 1; i >= 0; i--)
        {
            var model = CreateStructureModel(declarationsAncherPoints[i]);
            if (model != null)
            {
                result.Add(model);
            }
        }

        return result.ToImmutableList();
    }

    internal static BaseStructureModel? CreateStructureModel(SymbolAnchorPoint anchorPoint)
    {
        // format method name with parameters:
        var symbol = anchorPoint.Symbol;
        switch (symbol)
        {
            case IMethodSymbol methodSymbol:
                {
                    return new FunctionModel(
                        methodSymbol.Name == TopLevelStatementMainMethodName
                            ? Strings.TopLevelStatements!
                            : methodSymbol.ToDisplayString(SymbolFileStructureElementModel.MethodDisplayFormat),
                        VsImageHelper.GetImageMonikers(anchorPoint.Symbol.GetImageId()),
                        anchorPoint);
                }

            case IPropertySymbol { IsIndexer: true }:
                return new TypeMemberModel(
                    symbol.ToDisplayString(SymbolFileStructureElementModel.IndexerSymbolDisplayFormat),
                    VsImageHelper.GetImageMonikers(anchorPoint.Symbol.GetImageId()),
                    anchorPoint);

            case IPropertySymbol or IFieldSymbol or IEventSymbol:
                return new TypeMemberModel(
                    symbol.ToDisplayString(SymbolFileStructureElementModel.SymbolDisplayFormat),
                    VsImageHelper.GetImageMonikers(anchorPoint.Symbol.GetImageId()),
                    anchorPoint);

            case INamedTypeSymbol typeSymbol:
                {
                    var canHaveChildren = typeSymbol.TypeKind is TypeKind.Class or TypeKind.Interface or TypeKind.Struct
                                              or TypeKind.Enum or TypeKind.Module
                                          || typeSymbol.IsExtensionType();

                    return new TypeModel(
                        typeSymbol.ToDisplayString(SymbolFileStructureElementModel.SymbolDisplayFormat),
                        VsImageHelper.GetImageMonikers(anchorPoint.Symbol.GetImageId()),
                        anchorPoint,
                        typeSymbol.GetFullMetadataName()) { CanHaveChildren = canHaveChildren };
                }

            default:
                return null;
        }
    }
}
