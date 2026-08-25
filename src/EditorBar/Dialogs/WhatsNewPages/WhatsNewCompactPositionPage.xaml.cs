// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Windows.Controls;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Dialogs.WhatsNewPages;

public partial class WhatsNewCompactPositionPage : WhatsNewPageControl
{
    public WhatsNewCompactPositionPage()
    {
        this.InitializeComponent();
    }

    internal ImageBrush ArtworkBrush => this.FeatureArtworkImageBrush;

    internal Border ArtworkBorder => this.FeatureArtworkBorder;
}
