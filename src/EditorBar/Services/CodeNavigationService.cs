// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Text.RegularExpressions;
using System.Threading;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services;

/// <summary>
/// Handles navigation within code files, allowing users to jump to specific symbols or positions in the text
/// view.
/// </summary>
internal class CodeNavigationService : ICodeNavigationService, ITextNavigationService
{
    private readonly ITextView _textView;

    public CodeNavigationService(ITextView textView)
    {
        this._textView = Requires.NotNull(textView, nameof(textView));
    }

    public async Task NavigateToSymbolDeclarationInCurrentViewAsync(ISymbol symbol, Location? location)
    {
        if (symbol.DeclaringSyntaxReferences.Length == 0)
        {
            return;
        }

        SyntaxNode syntaxNode;
        if (symbol.DeclaringSyntaxReferences.Length == 1)
        {
            var declarationSyntax = symbol.DeclaringSyntaxReferences.FirstOrDefault();
            if (declarationSyntax == null)
            {
                return;
            }

            syntaxNode = await declarationSyntax.GetSyntaxAsync(CancellationToken.None);
        }
        else
        {
            var currentFilePath = this._textView.TextBuffer?.GetFileName();
            var inCurrentFile = symbol.DeclaringSyntaxReferences.Where(syntaxReference =>
                    PathUtils.Equals(syntaxReference.SyntaxTree.FilePath, currentFilePath))
                .ToArray();

            if (inCurrentFile.Length == 0)
            {
                syntaxNode = await symbol.DeclaringSyntaxReferences.First().GetSyntaxAsync(CancellationToken.None);
            }
            else if (inCurrentFile.Length == 1)
            {
                syntaxNode = await inCurrentFile.First().GetSyntaxAsync(CancellationToken.None);
                await this.NavigateToSyntaxNodeInCurrentViewAsync(syntaxNode);
                return;
            }
            else
            {
                syntaxNode = await inCurrentFile.First().GetSyntaxAsync(CancellationToken.None);
                if (location != null)
                {
                    foreach (var syntaxReference in inCurrentFile)
                    {
                        var tempSyntaxNode = await syntaxReference.GetSyntaxAsync(CancellationToken.None);
                        if (tempSyntaxNode.GetLocation().GetLineSpan().StartLinePosition.Line ==
                            location.GetLineSpan().StartLinePosition.Line)
                        {
                            syntaxNode = tempSyntaxNode;
                            break;
                        }
                    }
                }
            }
        }

        await this.NavigateToSyntaxNodeInCurrentViewAsync(syntaxNode);
    }

    public async Task NavigateToSyntaxNodeInCurrentViewAsync(SyntaxNode syntaxNode)
    {
        // Safely pattern-match to get the token for the identifier
        // You can expand this switch expression for other node types if needed
        var identifierToken = syntaxNode switch
        {
            MethodDeclarationSyntax methodDecl => methodDecl.Identifier,
            ConstructorDeclarationSyntax ctorDecl => ctorDecl.Identifier,
            PropertyDeclarationSyntax propertyDecl => propertyDecl.Identifier,
            EventDeclarationSyntax eventDecl => eventDecl.Identifier,
            FieldDeclarationSyntax fieldDecl => fieldDecl.Declaration.Variables.First().Identifier,
            EventFieldDeclarationSyntax eventField => eventField.Declaration.Variables.First().Identifier,
            TypeDeclarationSyntax typeDecl => typeDecl.Identifier, // classes, structs, records
            EnumDeclarationSyntax enumDecl => enumDecl.Identifier,
            DelegateDeclarationSyntax delegateDecl => delegateDecl.Identifier,
            // fallback: just pick the first token if none of the above
            _ => syntaxNode.GetFirstToken()
        };

        // The start of the identifier token in the overall file
        if (identifierToken != null)
        {
            // TODO: check the file path, if it's different, then we have to open new view with the target file
            var isSameFile = this.IsInCurrentFile(syntaxNode);
            if (isSameFile)
            {
                await this.NavigateToPositionAsync(identifierToken.SpanStart);
            }
            else
            {
                await NavigateToNewViewAsync(syntaxNode);
            }
        }
    }

    public async Task NavigateToAnchorAsync(AnchorPoint anchorPoint)
    {
        if (anchorPoint is SymbolAnchorPoint symbolAncherPoint)
        {
            await this.NavigateToSymbolAsync(symbolAncherPoint.Symbol, symbolAncherPoint.Location);
        }
        else
        {
            var file = anchorPoint.FilePath;
            var position = anchorPoint.TextSpan.Start;

            var view = await TryOpenDocumentViewAsync(file);
            if (view is { TextView: not null })
            {
                await NavigateToPositionInViewAsync(view.TextView, position);
            }
        }
    }

    public Task NavigateToPositionAsync(int position)
    {
        return NavigateToPositionInViewAsync(this._textView, position);
    }

    public Task NavigateToPositionAsync(int lineNumber, int columnNumber)
    {
        var snapshot = this._textView.TextSnapshot;
        var line = snapshot?.GetLineFromLineNumber(lineNumber);
        if (line != null)
        {
            var position = line.Start + columnNumber;
            return this.NavigateToPositionAsync(position);
        }

        return Task.CompletedTask;
    }

    public async Task NavigateToSymbolAsync(ISymbol symbol, Location location)
    {
        await this.NavigateToPositionAsync(location.SourceSpan.Start);
    }

    private static async Task NavigateToNewViewAsync(SyntaxNode token)
    {
        var file = token.SyntaxTree.FilePath;

        var isRazorView = TryMatchRazorView(file, out var razorViewFile);
        if (isRazorView)
        {
            file = razorViewFile!;
        }

        var view = await TryOpenDocumentViewAsync(file);

        if (view is { TextView: not null })
        {
            if (!isRazorView)
            {
                await view.TextView.GetCodeNavigationService().NavigateToSyntaxNodeInCurrentViewAsync(token);
            }

            // activate the view
            if (view.WindowFrame != null)
            {
                await view.WindowFrame.ShowAsync();
            }
        }
    }

    private static async Task<DocumentView?> TryOpenDocumentViewAsync(string file)
    {
        var isOpen = await VS.Documents.IsOpenAsync(file);
        if (!isOpen)
        {
            await VS.Documents.OpenAsync(file);
        }

        return await VS.Documents.GetDocumentViewAsync(file);
    }

    private static bool TryMatchRazorView(string file, out string? razorView)
    {
        // if the file path is for a generated file for Razor view, we have to unfuck it
        // for razor view  "<path>\something.razor" is the generated path "<path>\something.razor.<randomstring>.ide.g.<ext>"
        // <ext> is extension associated with the language, e.g. .cs for C# (it can be anything and we throw it away)

        // e.g. Index.razor.b4a-mOPQ32.ide.g.cs -> Index.razor

        razorView = file;

        if (string.IsNullOrEmpty(file) || !file.Contains("ide.g."))
        {
            return false;
        }

        // Separate path from filename
        var directory = Path.GetDirectoryName(file);
        var fileName = Path.GetFileName(file);

        const string pattern = @"^(?<razorViewName>.*\.razor)(?:\.[^.]+)*\.ide\.g\.[^.]+$";

        var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);
        if (match.Success)
        {
            // Rebuild the path with the "razor" portion
            var newFileName = match.Groups["razorViewName"]?.Value;
            razorView = string.IsNullOrEmpty(directory!)
                ? newFileName
                : Path.Combine(directory!, newFileName!);
            return true;
        }

        // If it doesn't match the Razor-generated pattern, just return as-is
        return false;
    }


    private bool IsInCurrentFile(SyntaxNode declarationSyntax)
    {
        var nodeFilePath = declarationSyntax.SyntaxTree.FilePath;
        var currentFilePath = this._textView.TextBuffer?.GetFileName();
        return PathUtils.Equals(nodeFilePath, currentFilePath);
    }

    private static Task NavigateToPositionInViewAsync(ITextView textView, int position)
    {
        if (textView.Caret == null)
        {
            return Task.CompletedTask;
        }

        var snapshot = textView.TextSnapshot!;
        textView.Caret.MoveTo(new SnapshotPoint(snapshot, position));
        textView.ViewScroller!.EnsureSpanVisible(new SnapshotSpan(snapshot, position, 1));
        (textView as IWpfTextView)?.VisualElement?.Focus();

        return Task.CompletedTask;
    }
}