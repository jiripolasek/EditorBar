// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JPSoftworks.EditorBar;

/// <summary>
/// Specialized class of editor bar factory for creating a margin on the top of the editor.
/// </summary>
/// <remarks>
/// We have to have two separate classes since there can't be multiple <see cref="MarginContainerAttribute" /> on a
/// class.
/// </remarks>
/// <seealso cref="JPSoftworks.EditorBar.BaseEditorBarFactory" />
[Export(typeof(IWpfTextViewMarginProvider))]
[Name("EditorBar-Top")]
[MarginContainer(PredefinedMarginNames.Top)]
[Order(After = PredefinedMarginNames.HorizontalScrollBar)]
[ContentType(StandardContentTypeNames.Text)]
[TextViewRole(PredefinedTextViewRoles.Document)]
class TopEditorBarFactory() : BaseEditorBarFactory(BarPosition.Top);