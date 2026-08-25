// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers.VisualStudio;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.PlatformUI;

namespace JPSoftworks.EditorBar.Dialogs;

public partial class WhatsNewDialog : DialogWindow
{
    private const string ChangelogUrl = "https://github.com/jiripolasek/EditorBar/blob/master/CHANGELOG.md";
    private const string LightHeaderArtworkFileName = "WhatsNewHeader.Light.png";
    private const string DarkHeaderArtworkFileName = "WhatsNewHeader.Dark.png";
    private const string CompactPositionArtworkFileName = "WhatsNewCompactPosition.png";
    private const string SearchResultsArtworkFileName = "WhatsNewSearchResults.png";

    // Update this value when the release version is finalized.
    private static readonly Version CurrentWhatsNewContentVersion = new(4, 1, 0);

    private readonly WhatsNewPageDefinition[] _pages;
    private readonly Border[] _pageIndicators;
    private int _pageIndex;

    internal bool OpenOptionsRequested { get; private set; }

    internal bool HasPages => this._pages.Length > 0;

    internal WhatsNewDialog(WhatsNewDisplayContext context)
    {
        this.InitializeComponent();
        this.LoadEditorBarIcon();
        this.LoadHeaderArtwork();
        this.LoadFeatureArtwork(
            this.CompactPositionPage.ArtworkBrush,
            this.CompactPositionPage.ArtworkBorder,
            CompactPositionArtworkFileName);
        this.LoadFeatureArtwork(
            this.SearchResultsPage.ArtworkBrush,
            this.SearchResultsPage.ArtworkBorder,
            SearchResultsArtworkFileName);

        this.Title = context.IsFirstRun ? $"Welcome to {Vsix.Name}" : $"What's New in {Vsix.Name}";
        this.DialogMonikerTextBlock.Text = this.Title.ToUpperInvariant();

        WhatsNewPageDefinition[] allPages =
        [
            new(
                this.BreadcrumbsPage,
                CurrentWhatsNewContentVersion,
                WhatsNewPageAudience.FirstRun),
            new(
                this.CompactPositionPage,
                CurrentWhatsNewContentVersion,
                WhatsNewPageAudience.Upgrade | WhatsNewPageAudience.Manual),
            new(
                this.SearchResultsPage,
                CurrentWhatsNewContentVersion,
                WhatsNewPageAudience.Upgrade | WhatsNewPageAudience.Manual),
            new(
                this.CustomizationPage,
                CurrentWhatsNewContentVersion,
                WhatsNewPageAudience.FirstRun)
        ];

        foreach (var page in allPages)
        {
            page.Content.Visibility = Visibility.Collapsed;
        }

        this._pages = allPages.Where(page => page.ShouldShow(context)).ToArray();
        this._pageIndicators = this.CreatePageIndicators();

        if (this.HasPages)
        {
            this.UpdatePage();
        }
    }

    protected override void OnDialogThemeChanged()
    {
        base.OnDialogThemeChanged();
        this.LoadHeaderArtwork();
        this.BreadcrumbsPage.UpdateDefaultColors();
    }

    private void LoadEditorBarIcon()
    {
        try
        {
            var iconPath = GetResourcePath("Icon.png");
            if (iconPath == null || !File.Exists(iconPath))
            {
                this.EditorBarIconImage.Visibility = Visibility.Collapsed;
                return;
            }

            this.EditorBarIconImage.Source = LoadBitmapImage(iconPath);
        }
        catch (Exception ex)
        {
            this.EditorBarIconImage.Visibility = Visibility.Collapsed;
            ex.Log();
        }
    }

    private void LoadHeaderArtwork()
    {
        try
        {
            var fileName = EditorAppearanceHelper.GetCurrentMode() == EditorColorMode.Dark
                ? DarkHeaderArtworkFileName
                : LightHeaderArtworkFileName;
            var artworkPath = GetResourcePath(fileName);
            if (artworkPath == null || !File.Exists(artworkPath))
            {
                this.HeaderArtworkBrush.ImageSource = null;
                this.HeaderArtworkBorder.Visibility = Visibility.Collapsed;
                return;
            }

            this.HeaderArtworkBrush.ImageSource = LoadBitmapImage(artworkPath);
            this.HeaderArtworkBorder.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            this.HeaderArtworkBrush.ImageSource = null;
            this.HeaderArtworkBorder.Visibility = Visibility.Collapsed;
            ex.Log();
        }
    }

    private void LoadFeatureArtwork(ImageBrush artworkBrush, Border artworkBorder, string fileName)
    {
        try
        {
            var artworkPath = GetResourcePath(fileName);
            if (artworkPath == null || !File.Exists(artworkPath))
            {
                artworkBrush.ImageSource = null;
                artworkBorder.Visibility = Visibility.Collapsed;
                return;
            }

            artworkBrush.ImageSource = LoadBitmapImage(artworkPath);
            artworkBorder.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            artworkBrush.ImageSource = null;
            artworkBorder.Visibility = Visibility.Collapsed;
            ex.Log();
        }
    }

    private static string? GetResourcePath(string fileName)
    {
        var assemblyDirectory = Path.GetDirectoryName(typeof(WhatsNewDialog).Assembly.Location);
        return assemblyDirectory == null ? null : Path.Combine(assemblyDirectory, "Resources", fileName);
    }

    private static BitmapImage LoadBitmapImage(string path)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(path, UriKind.Absolute);
        image.EndInit();
        image.Freeze();
        return image;
    }

    private Border[] CreatePageIndicators()
    {
        var indicators = new Border[this._pages.Length];
        for (var index = 0; index < indicators.Length; index++)
        {
            var indicator = new Border
            {
                Width = 18,
                Height = 4,
                Margin = index == indicators.Length - 1 ? default : new Thickness(0, 0, 5, 0),
                CornerRadius = new CornerRadius(2)
            };
            indicator.SetResourceReference(Border.BackgroundProperty, "EditorBarAccentBrush");
            this.PageIndicatorsPanel.Children.Add(indicator);
            indicators[index] = indicator;
        }

        return indicators;
    }

    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (this._pageIndex == 0)
        {
            return;
        }

        this._pageIndex--;
        this.UpdatePage();
    }

    private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }

    private void CaptionCloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void NextButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (this._pageIndex == this._pages.Length - 1)
        {
            this.DialogResult = true;
            return;
        }

        this._pageIndex++;
        this.UpdatePage();
    }

    private void OpenOptionsButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.OpenOptionsRequested = true;
        this.DialogResult = true;
    }

    private void ViewChangelogButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Microsoft.VisualStudio.Shell.VsShellUtilities.OpenSystemBrowser(ChangelogUrl);
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    private void UpdatePage()
    {
        for (var index = 0; index < this._pages.Length; index++)
        {
            this._pages[index].Content.Visibility =
                index == this._pageIndex ? Visibility.Visible : Visibility.Collapsed;
        }

        var currentPage = this._pages[this._pageIndex];
        this.HeadingTextBlock.Text = currentPage.Heading;
        this.SubheadingTextBlock.Text = currentPage.Subheading;
        this.PageStatusTextBlock.Text = $"Page {this._pageIndex + 1} of {this._pages.Length}";

        this.BackButton.Visibility = this._pageIndex == 0 ? Visibility.Hidden : Visibility.Visible;
        this.OpenOptionsButton.Visibility =
            this._pageIndex == this._pages.Length - 1 ? Visibility.Visible : Visibility.Collapsed;
        this.NextButton.Content = this._pageIndex == this._pages.Length - 1 ? "_Done" : "_Next";

        for (var index = 0; index < this._pageIndicators.Length; index++)
        {
            this._pageIndicators[index].Opacity = index == this._pageIndex ? 1 : 0.35;
        }
    }
}
