// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.ViewModels;

/// <summary>
/// Represents a view model for the editor segment options.
/// </summary>
/// <remarks>
/// This class provides properties for the foreground color, background color, and visibility of the editor segment.
/// </remarks>
public class EditorSegmentOptionsViewModel : INotifyPropertyChanged
{
    private EditorColorMode _activeColorMode;
    private Color _darkBackgroundColor;
    private Color _darkForegroundColor;
    private bool _isVisible;
    private Color _lightBackgroundColor;
    private Color _lightForegroundColor;

    public event PropertyChangedEventHandler? PropertyChanged;

    public EditorColorMode ActiveColorMode
    {
        get => this._activeColorMode;
        set
        {
            if (this.SetProperty(ref this._activeColorMode, value))
            {
                this.RaisePropertyChanged(nameof(this.ForegroundColor));
                this.RaisePropertyChanged(nameof(this.BackgroundColor));
            }
        }
    }

    public Color LightForegroundColor
    {
        get => this._lightForegroundColor;
        set
        {
            if (this.SetProperty(ref this._lightForegroundColor, value) && this.ActiveColorMode == EditorColorMode.Light)
            {
                this.RaisePropertyChanged(nameof(this.ForegroundColor));
            }
        }
    }

    public Color DarkForegroundColor
    {
        get => this._darkForegroundColor;
        set
        {
            if (this.SetProperty(ref this._darkForegroundColor, value) && this.ActiveColorMode == EditorColorMode.Dark)
            {
                this.RaisePropertyChanged(nameof(this.ForegroundColor));
            }
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the editor segment.
    /// </summary>
    public Color ForegroundColor
    {
        get => this.ActiveColorMode == EditorColorMode.Light ? this.LightForegroundColor : this.DarkForegroundColor;
        set
        {
            if (this.ActiveColorMode == EditorColorMode.Light)
            {
                this.LightForegroundColor = value;
            }
            else
            {
                this.DarkForegroundColor = value;
            }
        }
    }

    public Color LightBackgroundColor
    {
        get => this._lightBackgroundColor;
        set
        {
            if (this.SetProperty(ref this._lightBackgroundColor, value) && this.ActiveColorMode == EditorColorMode.Light)
            {
                this.RaisePropertyChanged(nameof(this.BackgroundColor));
            }
        }
    }

    public Color DarkBackgroundColor
    {
        get => this._darkBackgroundColor;
        set
        {
            if (this.SetProperty(ref this._darkBackgroundColor, value) && this.ActiveColorMode == EditorColorMode.Dark)
            {
                this.RaisePropertyChanged(nameof(this.BackgroundColor));
            }
        }
    }

    /// <summary>
    /// Gets or sets the background color of the editor segment.
    /// </summary>
    public Color BackgroundColor
    {
        get => this.ActiveColorMode == EditorColorMode.Light ? this.LightBackgroundColor : this.DarkBackgroundColor;
        set
        {
            if (this.ActiveColorMode == EditorColorMode.Light)
            {
                this.LightBackgroundColor = value;
            }
            else
            {
                this.DarkBackgroundColor = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the editor segment is visible.
    /// </summary>
    public bool IsVisible
    {
        get => this._isVisible;
        set => this.SetProperty(ref this._isVisible, value);
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        this.RaisePropertyChanged(propertyName);
        return true;
    }

    private void RaisePropertyChanged(string? propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
