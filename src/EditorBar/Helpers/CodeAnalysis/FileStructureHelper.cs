// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Threading;
using JPSoftworks.EditorBar.Resources;
using JPSoftworks.EditorBar.Services.StructureProviders;
using JPSoftworks.EditorBar.Services.StructureProviders.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Helpers;

internal static class FileStructureHelper
{
    internal const string TopLevelStatementMainMethodName = "<Main>$";

    /// <summary>
    /// Finds the nearest semantic symbol under (or near) the given caret position,
    /// walking up ancestor nodes if necessary. Also promotes property accessors
    /// (e.g., get/set methods) to the actual property symbol.
    /// </summary>
    /// <param name="semanticModel">A fresh SemanticModel for the current Document.</param>
    /// <param name="root">SyntaxRoot of the Document.</param>
    /// <param name="position">Caret position in the source text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The nearest symbol if found, otherwise null.</returns>
    private static ImmutableArray<SymbolAnchorPoint> FindDeclarationsUnderPosition(
        SemanticModel semanticModel,
        SyntaxNode root,
        int position,
        CancellationToken cancellationToken)
    {
        var declarations = new List<SymbolAnchorPoint>();

        // 1) Find the token at (or just before) the specified position.
        var token = root.FindToken(position, true);
        if (token == default)
        {
            return ImmutableArray<SymbolAnchorPoint>.Empty; // empty list
        }

        // 2) Walk upward through the syntax node ancestors, looking for declared symbols
        foreach (var node in token.Parent?.AncestorsAndSelf() ?? [])
        {
            // Try to get a declared symbol (method, property, class, etc.)
            var declaredSymbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);
            if (declaredSymbol is not null)
            {
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

                    declarations.Add(new SymbolAnchorPoint(declaredSymbol, node.GetLocation()));
                }
            }
        }

        return declarations.ToImmutableArray();
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
        // return descendants that are not types, but can contain types - root + namespaces
        SyntaxNode[] roots = [root, .. root?.DescendantNodes().Where(static t => t is BaseNamespaceDeclarationSyntax)];
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

        // Find the token under (or just before) the caret
        var token = syntaxRoot.FindToken(position);
        if (token == default)
        {
            return ImmutableList<BaseStructureModel>.Empty;
        }

        ;

        // Determine which symbol we’re on
        var declarationsAncherPoints =
            FindDeclarationsUnderPosition(semanticModel, syntaxRoot, position, cancellationToken);
        if (declarationsAncherPoints.Length == 0)
        {
            return ImmutableList<BaseStructureModel>.Empty;
        }

        ;

        // Manual reverse iteration for better performance
        var result = new List<BaseStructureModel>(declarationsAncherPoints.Length);
        for (var i = declarationsAncherPoints.Length - 1; i >= 0; i--)
        {
            var model = FormatSymbolName(declarationsAncherPoints[i]);
            if (model != null)
            {
                result.Add(model);
            }
        }

        return result.ToImmutableList();
    }

    private static BaseStructureModel? FormatSymbolName(SymbolAnchorPoint anchorPoint)
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
                return new TypeModel(
                    typeSymbol.ToDisplayString(SymbolFileStructureElementModel.SymbolDisplayFormat),
                    VsImageHelper.GetImageMonikers(anchorPoint.Symbol.GetImageId()),
                    anchorPoint,
                    typeSymbol.GetFullMetadataName())
                {
                    CanHaveChildren = typeSymbol.TypeKind is TypeKind.Class or TypeKind.Interface or TypeKind.Struct
                        or TypeKind.Enum or TypeKind.Module
                };

            default:
                return null;
        }
    }
}