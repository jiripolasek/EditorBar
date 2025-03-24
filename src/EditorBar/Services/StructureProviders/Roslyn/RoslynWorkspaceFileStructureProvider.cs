// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Threading;
using JPSoftworks.EditorBar.Helpers;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio.Text.Editor;
using CompilationUnitSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax;

namespace JPSoftworks.EditorBar.Services.StructureProviders.Roslyn;

/// <summary>
/// Provides file structure information for Roslyn workspace documents. It retrieves the structure based on the current
/// text view and caret position.
/// </summary>
internal class RoslynWorkspaceFileStructureProvider
{
    private readonly ITextView _textView;

    /// <summary>
    /// Initializes a new instance of the file structure provider for Roslyn workspaces.
    /// </summary>
    /// <param name="textView">Represents the text view associated with the workspace, used for displaying and editing code.</param>
    public RoslynWorkspaceFileStructureProvider(ITextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        this._textView = textView;
    }

    public async Task<StructureNavModel> GetFileStructureAsync()
    {
        var sourceTextContainer = this._textView.TextBuffer!.AsTextContainer();
        if (this._textView.TextBuffer == null || !Workspace.TryGetWorkspace(sourceTextContainer, out var workspace))
        {
            return new StructureNavModel(false, []);
        }

        // DocumentId can be tied to Document (which is of type Document) or to AdditionalDocument or AnalyzerConfigDocument (both are of type TextDocument)
        // Only Document can be parsed and analyzed by Roslyn, so we need to find the right type of document to work with

        var documentId = workspace.GetDocumentIdInCurrentContext(sourceTextContainer);
        var textDocument = workspace.CurrentSolution.GetDocument(documentId)
                           ?? workspace.CurrentSolution.GetAdditionalDocument(documentId)
                           ?? workspace.CurrentSolution.GetAnalyzerConfigDocument(documentId);

        if (textDocument == null || string.IsNullOrWhiteSpace(textDocument.FilePath!))
        {
            // now, let's panic
            return new StructureNavModel(false, []);
        }

        var document = textDocument as Document;
        var supportsCodeAnalysis = document is { SupportsSyntaxTree: true, SupportsSemanticModel: true };
        var canHaveItems = supportsCodeAnalysis && this._textView.Caret != null;

        // 2) get symbol under the caret
        IEnumerable<BaseStructureModel> breadcrumbs = canHaveItems
            ? await FileStructureHelper.GetStructureBreadcrumbsAsync(
                this._textView.Caret!.Position,
                document!,
                CancellationToken.None)
            : [];

        return new StructureNavModel(canHaveItems, breadcrumbs);
    }

    public async Task<ImmutableList<FileStructureElementModel>> GetChildItemsAsync(BaseStructureModel parentModel)
    {
        var sourceTextContainer = this._textView.TextBuffer!.AsTextContainer();
        if (this._textView.TextBuffer == null || !Workspace.TryGetWorkspace(sourceTextContainer, out var workspace))
        {
            return ImmutableList<FileStructureElementModel>.Empty;
        }

        var documentId = workspace.GetDocumentIdInCurrentContext(sourceTextContainer);
        if (documentId == null)
        {
            return ImmutableList<FileStructureElementModel>.Empty;
        }

        return parentModel switch
        {
            FileModel fileModel => await GetFileContentAsync(fileModel, workspace, documentId),
            TypeModel typeModel => await GetChildrenInTypeAsync(typeModel, workspace, documentId),
            _ => ImmutableList<FileStructureElementModel>.Empty
        };
    }

    private static async Task<ImmutableList<FileStructureElementModel>> GetFileContentAsync(
        FileModel fileModel,
        Workspace workspace,
        DocumentId documentId)
    {
        // filter out locations in the another file; we still can get multiple instances of the same symbol in the same file
        // this is different from how members are filtered, but until I have better UI for partial types it's the best solution

        var types = (await GetAllTypesInDocumentAsync(workspace, documentId))
            .Distinct(SymbolEqualityComparer.Default)
            .Where(FilterSourceMembers)
            .Where(FilterOutIdeGenerated);

        var result = types
            .SelectMany(
                symbol => symbol.Locations.Where(LocationIsInThisDocument),
                static (symbol, location) => new SymbolFileStructureElementModel(symbol, location)
                {
                    NavigationAction = textView =>
                        textView.GetCodeNavigationService()
                            .NavigateToSymbolDeclarationInCurrentViewAsync(symbol, location)
                })
            .Cast<FileStructureElementModel>()
            .ToImmutableList();

        return result;

        bool LocationIsInThisDocument(Location location)
        {
            return location.IsInSource &&
                   PathUtils.Equals(location.SourceTree?.FilePath, fileModel.AnchorPoint.FilePath);
        }
    }

    private static bool FilterOutIdeGenerated(ISymbol symbol)
    {
        // For Razor views we have to reaaaaly specific. I've no idea how to do this better.
        // Razor view files are generated by the Razor compiler and are not part of the solution. The file path doesn't actually exists in the file system.
        // Unfortunatelly the real code from *.razor file is mixed with generated code (like __o, __RazorDirectiveTokenHelpers__, BuildRenderTree, etc.).

        // 1) Filter out entirely if is in razor generated file (ide.g.) && has specific name:
        //     - __RazorDirectiveTokenHelpers__
        //     - __o
        //     - BuildRenderTree

        if (symbol is IMethodSymbol or IFieldSymbol && symbol.Locations.Length == 1 &&
            symbol.Locations[0].SourceTree?.FilePath.Contains("ide.g.") == true)
        {
            if (symbol.Name is "__RazorDirectiveTokenHelpers__" or "__o" or "BuildRenderTree")
            {
                return false;
            }
        }

        return true;
    }

    private static async Task<ImmutableList<FileStructureElementModel>> GetChildrenInTypeAsync(
        TypeModel typeModel,
        Workspace workspace,
        DocumentId documentId)
    {
        var typeSymbol = await FindSymbolAsync(typeModel.FullName, workspace, documentId);
        if (typeSymbol == null)
        {
            return ImmutableList<FileStructureElementModel>.Empty;
        }

        var members = typeSymbol.GetMembers()
            .Where(FilterSourceMembers)
            .Where(FilterOutIdeGenerated);

        return members
            .SelectMany(static member =>
            {
                if (member is IMethodSymbol { IsPartialDefinition: true } methodSymbol)
                {
                    // add both parts of the partial method (if present)
                    var parts = new List<ISymbol> { methodSymbol };
                    if (methodSymbol.PartialDefinitionPart != null &&
                        !SymbolEqualityComparer.Default.Equals(methodSymbol, methodSymbol.PartialDefinitionPart))
                    {
                        parts.Add(methodSymbol.PartialDefinitionPart);
                    }

                    if (methodSymbol.PartialImplementationPart != null &&
                        !SymbolEqualityComparer.Default.Equals(methodSymbol, methodSymbol.PartialImplementationPart))
                    {
                        parts.Add(methodSymbol.PartialImplementationPart);
                    }

                    return parts;
                }

                return [member];
            })
            .SelectMany(
                static member => member.Locations,
                static (symbol, location) => new SymbolFileStructureElementModel(symbol, location)
                {
                    NavigationAction = view =>
                        view.GetCodeNavigationService().NavigateToSymbolDeclarationInCurrentViewAsync(symbol)
                })
            .Cast<FileStructureElementModel>()
            .ToImmutableList();
    }

    private static bool FilterSourceMembers(ISymbol member)
    {
        // (1) Exclude compiler-synthesized members (like auto-property backing fields)
        if (member.IsImplicitlyDeclared)
        {
            return false;
        }

        // (2) Only keep members declared in source
        if (!member.Locations.Any(static loc => loc.IsInSource))
        {
            return false;
        }

        // (3) Exclude getters/setters and add/remove event methods
        if (member is IMethodSymbol methodSymbol)
        {
            switch (methodSymbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                case MethodKind.EventAdd:
                case MethodKind.EventRemove:
                    return false;
            }
        }

        // Passed all checks
        return true;
    }

    private static async Task<INamedTypeSymbol?> FindSymbolAsync(
        string metadataName,
        Workspace workspace,
        DocumentId documentId)
    {
        return await FileStructureHelper.GetSymbolByNameAsync(workspace, documentId, metadataName);
    }

    private static async Task<IList<ISymbol>> GetAllTypesInDocumentAsync(Workspace workspace, DocumentId documentId)
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

        var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

        if (document.Project.Language == LanguageNames.CSharp)
        {
            // detect top level statements file first
            if (root is CompilationUnitSyntax compilationUnit &&
                compilationUnit.Members.FirstOrDefault() is GlobalStatementSyntax firstGlobalStatementSyntax)
            {
                // this is top level statements file, we don't want to show types here, only method
                return semanticModel.GetEnclosingSymbol(firstGlobalStatementSyntax.SpanStart) is not IMethodSymbol
                    topLevelMainMethodSymbol
                    ? []
                    : [topLevelMainMethodSymbol];
            }

            // get document types
            var types = GetNonTypeTypeContainers(root).Where(static syntaxNode =>
                syntaxNode is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax);
            var symbols = types.Select(syntaxNode => semanticModel.GetDeclaredSymbol(syntaxNode)).OfType<ISymbol>()
                .ToList();

            return symbols;
        }
        else if (document.Project.Language == LanguageNames.VisualBasic)
        {
            // Ensure the document is a VB.NET file by trying to cast the root.
            if (root is not Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)
            {
                return [];
            }

            // Retrieve all descendant nodes that represent VB.NET type declarations.
            // In VB.NET, these include classes, structures, interfaces, modules, enums, and delegates.
            var typeNodes = root.DescendantNodes().Where(static node =>
                node is TypeBlockSyntax or EnumBlockSyntax or DelegateStatementSyntax);

            // Use the semantic model to retrieve the declared symbol for each type node.
            var symbols = typeNodes.Select(node => semanticModel.GetDeclaredSymbol(node))
                .OfType<ISymbol>()
                .ToList();

            return symbols;
        }

        return [];
    }

    private static IEnumerable<SyntaxNode> GetNonTypeTypeContainers(SyntaxNode? rootNode)
    {
        if (rootNode == null)
        {
            return [];
        }

        // return descendants that are not types, but can contain types - root + namespaces
        SyntaxNode[] roots =
            [rootNode, .. rootNode.DescendantNodes().Where(static t => t is BaseNamespaceDeclarationSyntax)];
        return roots.SelectMany(static t =>
            t.ChildNodes().Where(static node => node is not BaseNamespaceDeclarationSyntax));
    }
}