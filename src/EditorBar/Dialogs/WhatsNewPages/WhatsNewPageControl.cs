// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Windows.Controls;

namespace JPSoftworks.EditorBar.Dialogs.WhatsNewPages;

public class WhatsNewPageControl : UserControl
{
    public string Heading { get; set; } = string.Empty;

    public string Subheading { get; set; } = string.Empty;
}
