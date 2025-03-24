// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Controls;
using JPSoftworks.EditorBar.Helpers;
using Microsoft.VisualStudio.Imaging;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for IconDisplay.xaml
/// </summary>
public partial class IconDisplay : UserControl
{
    /// <summary>
    /// Registers a dependency property for the UIElement-based icon.
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(UIElement),
            typeof(IconDisplay),
            new PropertyMetadata(null, OnIconOrMonikerChanged));

    /// <summary>
    /// Registers a dependency property for the stacked ImageMoniker.
    /// </summary>
    public static readonly DependencyProperty ImageMonikerProperty =
        DependencyProperty.Register(
            nameof(ImageMoniker),
            typeof(StackedImageMoniker?),
            typeof(IconDisplay),
            new PropertyMetadata(null, OnIconOrMonikerChanged));

    /// <summary>
    /// A simple static cache of UI for given monikers.
    /// Key = StackedImageMoniker, Value = the constructed UIElement.
    /// </summary>
    private static readonly Dictionary<StackedImageMoniker, UIElement> s_monikerCache = [];

    /// <summary>
    /// If this is set, we show it directly. Otherwise, we fall back to ImageMoniker.
    /// </summary>
    public UIElement? Icon
    {
        get => (UIElement?)this.GetValue(IconProperty);
        set => this.SetValue(IconProperty, value);
    }

    /// <summary>
    /// If <see cref="Icon" /> is null, we create CrispImage or a grid of CrispImage
    /// for the monikers.
    /// </summary>
    public StackedImageMoniker? ImageMoniker
    {
        get => (StackedImageMoniker?)this.GetValue(ImageMonikerProperty);
        set => this.SetValue(ImageMonikerProperty, value);
    }

    public IconDisplay()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Called whenever either Icon or ImageMoniker changes.
    /// </summary>
    private static void OnIconOrMonikerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IconDisplay iconDisplay)
        {
            iconDisplay.RefreshUI();
        }
    }

    /// <summary>
    /// Rebuilds the displayed content depending on Icon / ImageMoniker.
    /// </summary>
    private void RefreshUI()
    {
        // The PART_Root is our layout container from the XAML.
        var rootGrid = (Grid)this.FindName("PART_Root");
        rootGrid.Children.Clear();

        if (this.Icon == null && (this.ImageMoniker == null || this.ImageMoniker.Value.IsEmpty))
        {
            this.Visibility = Visibility.Collapsed;
            return;
        }

        this.Visibility = Visibility.Visible;

        if (this.Icon != null)
        {
            // If Icon is explicitly set, display it directly.
            rootGrid.Children.Add(this.Icon);
            return;
        }

        if (this.ImageMoniker is StackedImageMoniker actualMoniker)
        {
            rootGrid.Children.Add(this.BuildMonikerUI(actualMoniker));
        }
    }

    /// <summary>
    /// Builds a CrispImage-based UI for the given moniker(s).
    /// If there's a single moniker, return one CrispImage.
    /// If two monikers, return a Grid with both CrispImage elements overlapping.
    /// If more, same approach but a Grid with multiple CrispImages.
    /// This can be styled or arranged further (e.g., offsets, margin, etc.).
    /// </summary>
    private UIElement BuildMonikerUI(StackedImageMoniker moniker)
    {
        if (moniker.IsSingle)
        {
            // Single CrispImage
            return new CrispImage { Moniker = moniker.Single };
        }

        if (moniker.IsPair)
        {
            // Overlap two CrispImages in a Grid
            var grid = new Grid();
            var first = new CrispImage { Moniker = moniker.First };
            var second = new CrispImage { Moniker = moniker.Second };

            // Add them in order so second is rendered on top
            grid.Children.Add(first);
            grid.Children.Add(second);
            return grid;
        }
        else
        {
            // Overlap all CrispImages in a grid
            var grid = new Grid();
            var all = moniker.Monikers; // 3+ items
            foreach (var m in all)
            {
                var crispImage = new CrispImage { Moniker = m };
                grid.Children.Add(crispImage);
            }

            return grid;
        }
    }
}