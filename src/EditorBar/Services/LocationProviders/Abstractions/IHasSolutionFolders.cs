// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents a solution that has solution folders.
/// </summary>
public interface IHasSolutionFolders
{
    /// <summary>
    /// Gets the list of solution folders.
    /// </summary>
    IReadOnlyList<string> SolutionFolders { get; }
}