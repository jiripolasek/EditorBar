// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Threading;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Provides a way to create a <see cref="LocationNavModel" /> for the current location in the editor.
/// </summary>
public interface ILocationProvider
{
    /// <summary>
    /// Creates new location navigation model for the current location in the editor.
    /// </summary>
    /// <param name="wpfTextView">The WPF text view.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="wpfTextView" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown back at the awaiting caller if <paramref name="cancellationToken" /> is canceled, even if the caller is already on the main thread.
    /// </exception>
    /// <returns>
    /// Task that represents the asynchronous operation. Task result contains the location navigation model or <see langword="null" /> if the location is not available.
    /// </returns>
    Task<LocationNavModel?> CreateAsync(IWpfTextView wpfTextView, CancellationToken cancellationToken = default);
}