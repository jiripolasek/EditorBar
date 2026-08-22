// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JPSoftworks.EditorBar.MefComponents;

/// <summary>
/// Specialized class of editor bar factory for creating a margin beside the horizontal scroll bar.
/// </summary>
/// <remarks>
/// We have to have separate classes since there can't be multiple <see cref="MarginContainerAttribute" /> on a
/// class.
/// </remarks>
/// <seealso cref="BaseEditorBarFactory" />
[Export(typeof(IWpfTextViewMarginProvider))]
[Name(EditorBarMarginNames.BottomControl)]
[Order(After = PredefinedMarginNames.ZoomControl)]
[MarginContainer(PredefinedMarginNames.BottomControl)]
[ContentType(StandardContentTypeNames.Text)]
[TextViewRole(PredefinedTextViewRoles.Document)]
internal class BottomControlEditorBarFactory()
    : BaseEditorBarFactory(BarPosition.BottomControl, EditorBarMarginNames.BottomControl);
