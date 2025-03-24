// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;

namespace JPSoftworks.EditorBar.Presentation;

/// <summary>
/// Converts a boolean value to a visibility value, with the inverted logic.
/// </summary>
internal sealed class InvertedBooleanToVisibilityConverter()
    : BooleanConverter<Visibility>(Visibility.Hidden, Visibility.Visible);