// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Windows.Controls;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers.Presentation;
using JPSoftworks.EditorBar.Helpers.VisualStudio;
using JPSoftworks.EditorBar.Options;
using DrawingColor = System.Drawing.Color;

namespace JPSoftworks.EditorBar.Dialogs.WhatsNewPages;

public partial class WhatsNewBreadcrumbsPage : WhatsNewPageControl
{
    public WhatsNewBreadcrumbsPage()
    {
        this.InitializeComponent();
        this.UpdateDefaultColors();
    }

    internal void UpdateDefaultColors()
    {
        var defaultColors = new GeneralOptionsModel().GetColorSet(EditorAppearanceHelper.GetCurrentMode());

        SetColors(this.SolutionPreviewButton, defaultColors.SolutionBackground, defaultColors.SolutionForeground);
        SetColors(
            this.SolutionFolderPreviewButton,
            defaultColors.SolutionFolderBackground,
            defaultColors.SolutionFolderForeground);
        SetColors(this.ProjectPreviewButton, defaultColors.ProjectBackground, defaultColors.ProjectForeground);
        SetColors(
            this.ProjectFolderPreviewButton,
            defaultColors.ProjectFoldersBackground,
            defaultColors.ProjectFoldersForeground);
        SetColors(
            this.FilePreviewButton,
            defaultColors.FileBreadcrumbBackground,
            defaultColors.FileBreadcrumbForeground);
        SetColors(
            this.ClassPreviewButton,
            defaultColors.StructureBreadcrumbBackground,
            defaultColors.StructureBreadcrumbForeground);
        SetColors(
            this.MethodPreviewButton,
            defaultColors.StructureBreadcrumbBackground,
            defaultColors.StructureBreadcrumbForeground);
    }

    private static void SetColors(Button button, DrawingColor background, DrawingColor foreground)
    {
        button.Background = CreateBrush(background);
        button.Foreground = CreateBrush(foreground);
    }

    private static SolidColorBrush CreateBrush(DrawingColor color)
    {
        var brush = new SolidColorBrush(color.ToMediaColor());
        brush.Freeze();
        return brush;
    }
}
