// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.IO;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar;

/// <summary>
/// Represents margin for <see cref="IWpfTextView" />. It is used to display <see cref="EditorBarControl" /> in the
/// editor.
/// </summary>
public class EditorBarMargin : IWpfTextViewMargin
{
    // collection of text view roles that are used in diff views
    private static readonly string[] DiffViews =
    [
        DifferenceViewerRoles.DiffTextViewRole,
        DifferenceViewerRoles.LeftViewTextViewRole,
        DifferenceViewerRoles.RightViewTextViewRole,
        DifferenceViewerRoles.InlineViewTextViewRole
    ];

    private readonly EditorBarControl _editorBarControl;
    private readonly IWpfTextView _textView;
    private readonly BarPosition _position;
    private readonly IServiceProvider _serviceProvider;
    private readonly JoinableTaskFactory _joinableTaskFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorBarMargin" /> class.
    /// </summary>
    /// <param name="textView">The text view.</param>
    /// <param name="joinableTaskFactory">A factory for starting asynchronous tasks that can mitigate deadlocks.</param>
    /// <param name="serviceProvider"></param>
    /// <param name="position">The position.</param>
    public EditorBarMargin(IWpfTextView textView, JoinableTaskFactory joinableTaskFactory,
        IServiceProvider serviceProvider, BarPosition position)
    {
        this._textView = textView;
        this._position = position;
        this._joinableTaskFactory = joinableTaskFactory;
        this._editorBarControl = new EditorBarControl(textView, joinableTaskFactory);
        this._serviceProvider = serviceProvider;

        GeneralOptionsModel.Saved += this.GeneralPageOnSaved;
        this.GeneralPageOnSaved(GeneralOptionsModel.Instance);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        GeneralOptionsModel.Saved -= this.GeneralPageOnSaved;
        this._editorBarControl.Dispose();
    }

    /// <summary>
    /// Gets the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> with the given
    /// <paramref name="marginName" />.
    /// </summary>
    /// <param name="marginName">The name of the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" />.</param>
    /// <returns>
    /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> named <paramref name="marginName" />, or
    /// null if no match is found.
    /// </returns>
    /// <remarks>
    /// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
    /// forwards the call to its children. Margin name comparisons are case-insensitive.
    /// </remarks>
    public ITextViewMargin? GetTextViewMargin(string marginName)
    {
        return marginName == "EditorBar-" + this._position ? this : null;
    }

    /// <summary>
    /// Gets the size of the margin.
    /// </summary>
    /// <remarks>
    /// For a horizontal margin this is the height of the margin,
    /// since the width will be determined by the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.
    /// For a vertical margin this is the width of the margin, since the height will be determined by the
    /// <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.
    /// </remarks>
    public double MarginSize => this.Enabled ? this._editorBarControl.ActualHeight : 0;

    /// <summary>
    /// Determines whether the margin is enabled.
    /// </summary>
    public bool Enabled { get; private set; }

    /// <summary>
    /// Gets the <see cref="T:System.Windows.FrameworkElement" /> that renders the margin.
    /// </summary>
    public FrameworkElement VisualElement => this._editorBarControl;

    private void GeneralPageOnSaved(GeneralOptionsModel generalOptionsModel)
    {
        this.Enabled = generalOptionsModel.Enabled && generalOptionsModel.BarPosition == this._position;
        this._editorBarControl.Visibility = this.Enabled && this.IsSupported(generalOptionsModel) ? Visibility.Visible : Visibility.Collapsed;
    }

    private bool IsSupported(GeneralOptionsModel generalOptionsModel)
    {
        /*
         Document view roles
        --------------------
         Main document view:
            DEBUGGABLE,PRIMARYDOCUMENT,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,STRUCTURED,ZOOMABLE,UBIDIFF,UBIRIGHTDIFF
         Diff view:
            DIFF,RIGHTDIFF,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,ZOOMABLE,PRIMARYDOCUMENT
         GIT diff:
            DIFF,RIGHTDIFF,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,ZOOMABLE,LINESTAGINGVIEWROLE,SHOWDIFFACTIONSTOOLBARROLE,PRIMARYDOCUMENT
            DIFF,LEFTDIFF,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,ZOOMABLE,LINESTAGINGVIEWROLE,SHOWDIFFACTIONSTOOLBARROLE
            DIFF,INLINEDIFF,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,ZOOMABLE,LINESTAGINGVIEWROLE,SHOWDIFFACTIONSTOOLBARROLE,PRIMARYDOCUMENT
         Preview:
            DEBUGGABLE,PRIMARYDOCUMENT,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,STRUCTURED,ZOOMABLE,UBIDIFF,UBIRIGHTDIFF
         History:
            DIFF,RIGHTDIFF,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,ZOOMABLE,DOCUMENT,INTERACTIVE,ZOOMABLE,PRIMARYDOCUMENT
        copilot:
            DEBUGGABLE,PRIMARYDOCUMENT,DIFFERENCEVIEWERWITHADAPTERSROLE,ANALYZABLE,DOCUMENT,EDITABLE,INTERACTIVE,ZOOMABLE,DIFF,INLINEDIFF
        Blame:
            DOCUMENT,EDITABLE,INTERACTIVE,ZOOMABLE,ANNOTATE
            # blame is a temp file

        Non-document view roles
        -----------------------
        peek definition:
            EDITABLE,ANALYZABLE,STRUCTURED,INTERACTIVE,EMBEDDED_PEEK_TEXT_VIEW,TRANSPARENT_BACKGROUND

        Summary
        -------
        Lot of views have role "Primary document", so effectively we have to disallow elements we don't want.
         */

        try
        {
            var textViewRoleSet = this._textView.Roles;
            if (textViewRoleSet == null)
                return false;

            var isPrimaryDocument = textViewRoleSet.Contains(PredefinedTextViewRoles.PrimaryDocument);
            var isDocument = textViewRoleSet.Contains(PredefinedTextViewRoles.Document);
            var isStrictlyAuxiliaryDocument = isDocument && !isPrimaryDocument;
            var isEditable = textViewRoleSet.Contains(PredefinedTextViewRoles.Editable);
            var isInteractive = textViewRoleSet.Contains(PredefinedTextViewRoles.Interactive);
            var isDiffView = textViewRoleSet.ContainsAny(DiffViews);
            var isBlame = textViewRoleSet.Contains("ANNOTATE");

            if (!isInteractive)
                return false;

            // if its a diff view, force  show of the bar regardless of the other settings
            // there are multiple text views in diff view, each have different roles
            //
            // note: in left/right diff view, force bar on the both sides so the
            //   lines on both sides match with each other
            if (isDiffView)
                return generalOptionsModel.DisplayInDiffViews;

            if (!isPrimaryDocument && !isDocument)
                return false;

            var fileStatus = this.GetFileReadOnlyStatus(this._textView);

            // if there's no file, don't show the bar
            var isGhost = string.IsNullOrWhiteSpace(fileStatus.FileName!);
            if (isGhost)
                return false;

            isEditable &= !fileStatus.IsReadOnly && !isGhost;

            var isTemp = !isGhost && fileStatus.FileName!.StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase);

            var checkBlame = !isBlame || (isBlame && generalOptionsModel.DisplayInBlame);

            // should we display bar for temp files?
            // - the blame file is a temp file, so ignore this rule in that case
            var checkTemp = isBlame || (!isTemp || (isTemp && generalOptionsModel.DisplayInTempFiles));

            // is strictly auxiliary document
            var checkAux = !isStrictlyAuxiliaryDocument || (isStrictlyAuxiliaryDocument && generalOptionsModel.DisplayInAuxiliaryDocuments);

            // is (non-)editable
            var checkNonEditable = isEditable || (!isEditable && generalOptionsModel.DisplayInNonEditableDocuments);
            
            return (isPrimaryDocument || checkAux) && !isDiffView && checkNonEditable && checkBlame && checkTemp;
        }
        catch (Exception ex)
        {
            ex.Log();
        }

        return false;
    }

    private DocumentStatus GetFileReadOnlyStatus(ITextView textView)
    {
        return this._joinableTaskFactory.Run(async () => await this.GetFileReadOnlyStatusCoreAsync(textView));
    }

    private async Task<DocumentStatus> GetFileReadOnlyStatusCoreAsync(ITextView textView)
    {
        await this._joinableTaskFactory.SwitchToMainThreadAsync();

        var vsTextView = textView.Properties.GetProperty<IVsTextView>(typeof(IVsTextView));
        if (vsTextView == null)
            return new DocumentStatus(string.Empty, false);

        try
        {
            var documentView = await vsTextView.ToDocumentViewAsync();
            if (documentView == null)
                return new DocumentStatus(string.Empty, false);

            var filePath = documentView.FilePath;
            if (filePath == null)
                return new DocumentStatus(string.Empty, false);

            if (IsBufferReadOnly(documentView.TextBuffer))
                return new DocumentStatus(filePath, true);

            if (this.IsDocumentReadOnly(filePath))
                return new DocumentStatus(filePath, true);

            return new DocumentStatus(filePath, false);
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Failed to determine readonly status of the document.");
            return new DocumentStatus(string.Empty, false);
        }
    }

    private static bool IsBufferReadOnly(ITextBuffer? textBuffer)
    {
        if (textBuffer == null)
            return false;

        try
        {
            return IsFullyReadOnly(textBuffer);
        }
        catch (Exception ex)
        {
            ex.LogAsync("Failed to check readonly status of the buffer.").FireAndForget();
            return false;
        }
    }

    private bool IsDocumentReadOnly(string filePath)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        try
        {
            if (this._serviceProvider.GetService(typeof(DTE)) is DTE2 dte)
            {
                foreach (Document doc in dte.Documents!)
                {
                    if (string.Equals(doc.FullName!, filePath, StringComparison.OrdinalIgnoreCase))
                        return doc.ReadOnly;
                }
            }
        }
        catch (Exception ex)
        {
            ex.LogAsync("Failed to check readonly status of the document in DTE.").FireAndForget();
        }

        return false;
    }


    private static bool IsFullyReadOnly(ITextBuffer textBuffer)
    {
        // Get the current snapshot of the text buffer
        var snapshot = textBuffer.CurrentSnapshot;
        if (snapshot == null)
        {
            return false; // No snapshot, so not fully read-only
        }

        // Check if the buffer has any read-only regions
        var readOnlyRegions = textBuffer.GetReadOnlyExtents(new SnapshotSpan(snapshot, 0, snapshot.Length));
        if (readOnlyRegions == null || readOnlyRegions.Count == 0)
        {
            return false; // No read-only regions, so not fully read-only
        }

        // Combine the spans of all read-only regions
        var combinedSpan = new Span(int.MaxValue, 0);
        foreach (var region in readOnlyRegions)
        {
            combinedSpan = Span.FromBounds(
                Math.Min(combinedSpan.Start, region.Start),
                Math.Max(combinedSpan.End, region.End)
            );
        }

        // Check if the combined read-only region covers the entire buffer
        return combinedSpan.Start == 0 && combinedSpan.End == snapshot.Length;
    }
}

internal readonly record struct DocumentStatus(string? FileName, bool IsReadOnly);