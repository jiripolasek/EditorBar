using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JPSoftworks.EditorBar;

[Export(typeof(IWpfTextViewMarginProvider))]
[Name("EditorBar-Top")]
[MarginContainer(PredefinedMarginNames.Top)]
[Order(After = PredefinedMarginNames.HorizontalScrollBar)]
[ContentType(StandardContentTypeNames.Text)]
[TextViewRole(PredefinedTextViewRoles.Document)]
public class TopEditorBarFactory() : BaseEditorBarFactory(BarPosition.Top);