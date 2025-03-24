// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Helpers;

internal static class VsImageHelper
{
    internal const int DefaultIconSize = 16;
    internal const int MiddleIconSize = 24;
    internal const int LargeIconSize = 32;
    internal const int XLargeIconSize = 48;

    public static StackedImageMoniker GetImageMonikers(int imageId)
    {
        if (imageId.HasOverlay())
        {
            return MakeOverlayMonikers(imageId);
        }

        var moniker = new ImageMoniker { Guid = KnownImageIds.ImageCatalogGuid, Id = imageId };
        return new StackedImageMoniker(moniker);
    }

    private static StackedImageMoniker MakeOverlayMonikers(int imageId)
    {
        var (baseImageId, overlayImageId, _) = imageId.DeconstructIconOverlay();
        var baseImage = new ImageMoniker { Guid = KnownImageIds.ImageCatalogGuid, Id = baseImageId };
        var overlay = new ImageMoniker { Guid = KnownImageIds.ImageCatalogGuid, Id = overlayImageId };
        return new StackedImageMoniker(baseImage, overlay);
    }

    /// <summary>
    /// Gets a themed <see cref="Image" /> from a value defined in <see cref="KnownImageIds" />
    /// </summary>
    /// <param name="imageId">The image id.</param>
    /// <param name="size">The size of the image.</param>
    public static FrameworkElement GetImage(int imageId, double size = 0)
    {
        if (imageId.HasOverlay())
        {
            return MakeOverlayImage(imageId, size);
        }

        var moniker = new ImageMoniker { Guid = KnownImageIds.ImageCatalogGuid, Id = imageId };
        if (size < 1)
        {
            size = DefaultIconSize;
        }

        return new CrispImage { Moniker = moniker, Height = size, Width = size };
    }

    public static FrameworkElement GetImage(string monikerName, int size = 0)
    {
        return GetImage(KnownMonikerNameMap.Map.TryGetValue(monikerName, out var i) ? i : KnownImageIds.Blank, size);
    }

    public static TControl ReferenceCrispImageBackground<TControl>(this TControl target, ThemeResourceKey colorKey)
        where TControl : FrameworkElement
    {
        target.SetResourceReference(ImageThemingUtilities.ImageBackgroundColorProperty!, colorKey);
        return target;
    }

    public static void SetBackgroundForCrispImage(this DependencyObject target, Color color)
    {
        ImageThemingUtilities.SetImageBackgroundColor(target, color);
    }

    private static Grid MakeOverlayImage(int imageId, double size)
    {
        var (baseImageId, overlayImageId, fullOverlay) = imageId.DeconstructIconOverlay();
        var baseImage = new ImageMoniker { Guid = KnownImageIds.ImageCatalogGuid, Id = baseImageId };
        var overlay = new ImageMoniker { Guid = KnownImageIds.ImageCatalogGuid, Id = overlayImageId };
        if (size < 1)
        {
            size = DefaultIconSize;
        }

        return new Grid
        {
            Height = size,
            Width = size,
            Children =
            {
                new CrispImage { Moniker = baseImage, Height = size, Width = size },
                new CrispImage
                {
                    Moniker = overlay,
                    Height = fullOverlay ? size : size *= 0.6,
                    Width = size,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                }
            }
        };
    }

    private static class KnownMonikerNameMap
    {
        internal static readonly Dictionary<string, int> Map = CreateMap();

        private static Dictionary<string, int> CreateMap()
        {
            return typeof(KnownImageIds)
                .GetFields()
                .Where(static item => item.FieldType == typeof(int))
                .ToDictionary(
                    static t => t.Name,
                    static t => (int)t.GetValue(null!));
        }
    }
}