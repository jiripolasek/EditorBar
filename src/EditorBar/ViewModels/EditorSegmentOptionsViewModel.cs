// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Windows.Media;
using JPSoftworks.EditorBar.Fx;

namespace JPSoftworks.EditorBar.ViewModels;

/// <summary>
/// Represents a view model for the editor segment options.
/// </summary>
/// <remarks>
/// This class provides properties for the foreground color, background color, and visibility of the editor segment.
/// </remarks>
public class EditorSegmentOptionsViewModel : ViewModel
{
    private Color _foregroundColor;
    private Color _backgroundColor;
    private bool _isVisible;

    /// <summary>
    /// Gets or sets the foreground color of the editor segment.
    /// </summary>
    public Color ForegroundColor
    {
        get => this._foregroundColor;
        set => this.SetField(ref this._foregroundColor, value);
    }

    /// <summary>
    /// Gets or sets the background color of the editor segment.
    /// </summary>
    public Color BackgroundColor
    {
        get => this._backgroundColor;
        set => this.SetField(ref this._backgroundColor, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the editor segment is visible.
    /// </summary>
    public bool IsVisible
    {
        get => this._isVisible;
        set => this.SetField(ref this._isVisible, value);
    }
}
