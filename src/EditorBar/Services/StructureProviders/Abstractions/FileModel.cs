// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers;
using Microsoft;
using Microsoft.IO;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Represents a file model in the editor bar.
/// </summary>
public class FileModel : BaseStructureModel
{
    /// <summary>
    /// Constructs a FileModel instance using a name, an image representation, and a specific anchor point.
    /// </summary>
    /// <param name="displayName">Specifies the name associated with the file model.</param>
    /// <param name="imageMoniker">Represents the visual icon or image linked to the file model.</param>
    /// <param name="anchorPoint">Defines the reference point for positioning the file model.</param>
    private FileModel(string displayName, StackedImageMoniker imageMoniker, AnchorPoint anchorPoint)
        : base(displayName, imageMoniker, anchorPoint)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="FileModel" /> class with the specified path and a value indicating whether it can have children.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="canHaveChildren">A value indicating whether the file model can have children.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path" /> is null or whitespace.</exception>
    public static FileModel Create(string path, bool canHaveChildren)
    {
        Requires.NotNullOrWhiteSpace(path, nameof(path));
        string name = Path.GetFileName(path);
        Requires.NotNullOrWhiteSpace(name, nameof(name));

        return new FileModel(name, GetImageMonikerForFile(path), new AnchorPoint(path))
        {
            CanHaveChildren = canHaveChildren
        };
    }

    private static StackedImageMoniker GetImageMonikerForFile(string value)
    {
        if (Directory.Exists(value))
        {
            return KnownMonikers.FolderOpened;
        }

        if (!File.Exists(value))
        {
            return KnownMonikers.Document;
        }

        try
        {
            // TODO: smell, should be refactored to lazy init (?)
            //       does really getting the moniker needs to run on the UI thread as well... leaking, leaking

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            return Package.GetGlobalService(typeof(SVsImageService)) is IVsImageService2 imageService
                ? imageService.GetImageMonikerForFile(value)
                : KnownMonikers.Document;
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }
        catch (Exception ex)
        {
            ex.Log();
            return KnownMonikers.Document;
        }
    }
}