// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace JPSoftworks.EditorBar.Fx;

/// <summary>
/// Represents a base ViewModel that implements the INotifyPropertyChanged interface.
/// </summary>
public abstract class ViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        // this exception propagates as a plague
        // ReSharper disable once EventExceptionNotDocumented
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName!));
    }

    /// <summary>
    /// Sets the field to the given value and raises the PropertyChanged event if the field has changed.
    /// </summary>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <param name="field">The field to set.</param>
    /// <param name="value">The value to set the field to.</param>
    /// <param name="propertyName">The name of the property that changed.</param>
    /// <returns>True if the field was changed; otherwise, false.</returns>
    [NotifyPropertyChangedInvocator]
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        this.OnPropertyChanged(propertyName);
        return true;
    }
}
