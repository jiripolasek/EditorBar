// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services.StructureProviders.Xml.Models;
using Microsoft;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders.Xml;

internal sealed class XmlFileStructureProvider : StructuredDocumentStructureProvider<XDocument>
{
    public XmlFileStructureProvider(ITextView textTextView) : base(textTextView) { }

    protected override Task<XDocument> ParseDocumentAsync(string text, CancellationToken cancellationToken)
    {
        try
        {
            var parsedDocument = XDocument.Parse(text,
                LoadOptions.PreserveWhitespace | LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
            return Task.FromResult(parsedDocument);
        }
        catch (Exception)
        {
            return Task.FromResult(new XDocument());
        }
    }

    protected override Task<(IEnumerable<BaseStructureModel> structure, bool rootHasChildren)>
        GetFileStructureCoreAsync(
            int caretPosition,
            XDocument document,
            ITextSnapshot textSnapshot,
            string filePath,
            CancellationToken cancellationToken)
    {
        Requires.NotNull(textSnapshot, nameof(textSnapshot));
        Requires.NotNull(document, nameof(document));

        var xPathSegmentsArray = CalculateXPath(caretPosition, document, textSnapshot);
        var xPathSegments = xPathSegmentsArray!.AsSpan();
        if (xPathSegments == null)
        {
            return Task.FromResult<(IEnumerable<BaseStructureModel> structure, bool rootHasChildren)>(([], false));
        }

        var result = new BaseStructureModel[xPathSegments.Length];
        for (var index = 0; index < xPathSegments.Length; index++)
        {
            var segment = xPathSegments[index];
            var segments = xPathSegments.Slice(0, index + 1);
            var xmlStructureModel = new XmlNodeStructureModel(filePath, segment.ToDisplayString(), segments)
            {
                ImageMoniker = KnownMonikers.MarkupXML, CanHaveChildren = segment.CanHaveChildren
            };
            result[index] = xmlStructureModel;
        }

        return Task.FromResult<(IEnumerable<BaseStructureModel> structure, bool rootHasChildren)>((result, true));
    }

    protected override async Task<ImmutableList<FileStructureElementModel>> GetChildItemsCoreAsync(
        BaseStructureModel parentModel,
        XDocument document,
        ITextSnapshot textSnapshot,
        CancellationToken cancellationToken)
    {
        var path = parentModel switch
        {
            FileModel => [],
            XmlNodeStructureModel xmlStructureModel => xmlStructureModel.Path,
            _ => null
        };

        if (path is null)
        {
            return ImmutableList<FileStructureElementModel>.Empty;
        }

        try
        {
            // find element using parentXPath and return its children

            XContainer? parentContainer;
            if (path.Length == 0)
            {
                parentContainer = document;
            }
            else
            {
                parentContainer = XPathSelectElement(document, path) as XElement;
            }

            if (parentContainer != null)
            {
                List<FileStructureElementModel> items = [];
                if (parentContainer is XElement parentElement)
                {
                    var attributes = parentElement.Attributes();
                    var temp = attributes.Select(a => new FileStructureElementModel
                    {
                        ImageMoniker = KnownMonikers.XMLAttribute,
                        PrimaryName = a.Name.LocalName,
                        SearchText = a.Value,
                        SecondaryName = a.Value.CropSafe(25),
                        NavigationAction = _ => this.NavigateTo(a)
                    });
                    items.AddRange(temp);
                }

                // get all child elements of parentElement (across all namespaces)

                var elements = parentContainer.Elements();
                items.AddRange(elements.Select(e => new FileStructureElementModel
                {
                    ImageMoniker = KnownMonikers.MarkupXML,
                    PrimaryName = e.Name.LocalName,
                    SearchText = e.Name.LocalName,
                    SecondaryName = GetFirstAttributeNameAndValue(e),
                    NavigationAction = _ => this.NavigateTo(e)
                }));

                return items.ToImmutableList();
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }

        return ImmutableList<FileStructureElementModel>.Empty;
    }

    private static XObject? XPathSelectElement(XDocument doc, XPathSegment[] path)
    {
        var currentElement = doc as XObject;

        foreach (var segment in path)
        {
            if (currentElement is not XContainer element)
            {
                return null;
            }

            var name = segment.Name;
            var index = segment.Index;

            currentElement = index > 0
                ? element.Elements(name).ElementAtOrDefault(index)
                : element.Element(name);
        }

        return currentElement;
    }

    private static string? GetFirstAttributeNameAndValue(XElement? element)
    {
        if (element?.HasAttributes == false)
        {
            return null;
        }

        // wasting cycles :(
        // var index = parentElement.Elements(element.Name).ToList().IndexOf(element);

        var firstAttribute = element!.Attributes().First();
        return $"""
                {firstAttribute.Name.LocalName}="{firstAttribute.Value.CropSafe(25)}"
                """;
    }

    private void NavigateTo(XObject xmlObject)
    {
        if (xmlObject is IXmlLineInfo li && li.HasLineInfo())
        {
            this.TextView
                .GetTextNavigationService()
                .NavigateToPositionAsync(li.LineNumber - 1, li.LinePosition - 1)
                .FireAndForget();
        }
    }


    private static XPathSegment[]? CalculateXPath(int position, XDocument document, ITextSnapshot textSnapshot)
    {
        try
        {
            var element = FindElementAtPosition(document, position, textSnapshot);
            return GetXPath(element);
        }
        catch
        {
            return null;
        }
    }

    private static (int line, int column) GetLineColumnFromPosition(ITextSnapshot textSnapshot, int position)
    {
        // Grab the line that contains 'position'
        var snapshotLine = textSnapshot.GetLineFromPosition(position);

        if (snapshotLine is null)
        {
            return (0, 0);
        }

        // Convert to 1-based line
        var lineNumber = snapshotLine.LineNumber + 1;

        // Convert to 1-based column (distance from line start)
        var columnNumber = position - snapshotLine.Start.Position + 1;

        return (lineNumber, columnNumber);
    }


    private static XElement? FindElementAtPosition(
        XDocument document,
        int absoluteCaretPosition,
        ITextSnapshot textSnapshot)
    {
        if (document.Root is null)
        {
            return null;
        }

        // Convert caret absolute offset to line/column
        var (caretLine, caretCol) = GetLineColumnFromPosition(textSnapshot, absoluteCaretPosition);

        XElement? bestMatch = null;

        // Iterate over all elements in the doc (including the root)
        foreach (var element in document.Root.DescendantsAndSelf())
        {
            if (element is not IXmlLineInfo lineInfo || !lineInfo.HasLineInfo())
            {
                continue;
            }

            var startLine = lineInfo.LineNumber;
            var startCol = lineInfo.LinePosition;

            // We'll accept this element if it starts on or before the caret line/column
            // (caretLine > startLine) or (caretLine == startLine && caretCol >= startCol)
            if (caretLine > startLine
                || (caretLine == startLine && caretCol >= startCol))
            {
                // If we don't yet have a best match, or if this one starts later, pick it
                if (bestMatch == null)
                {
                    bestMatch = element;
                }
                else
                {
                    var bestLineInfo = (IXmlLineInfo)bestMatch;
                    var bestLine = bestLineInfo.LineNumber;
                    var bestCol = bestLineInfo.LinePosition;

                    // We pick the one that starts *after* the current best but still <= caret
                    // i.e., the "closest" in the textual sense
                    if (startLine > bestLine || (startLine == bestLine && startCol > bestCol))
                    {
                        bestMatch = element;
                    }
                }
            }
        }

        return bestMatch;
    }


    private static XPathSegment[] GetXPath(XElement? element)
    {
        if (element == null)
        {
            return [];
        }

        var segments = new List<XPathSegment>();
        while (element != null)
        {
            var prefix = element.GetPrefixOfNamespace(element.GetDefaultNamespace());
            var name = element.Name.ToString();
            var index = element.Parent?.Elements(element.Name).ToList().IndexOf(element) ?? 0;
            segments.Insert(0, new XPathSegment(name, index, prefix) { CanHaveChildren = true });
            element = element.Parent;
        }

        return [.. segments];
    }
}