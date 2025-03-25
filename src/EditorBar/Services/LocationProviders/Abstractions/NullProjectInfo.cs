// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Services.LocationProviders;

/// <summary>
/// Represents a project with no information.
/// </summary>
internal sealed class NullProjectInfo : IProjectInfo
{
    /// <inheritdoc />
    public string DisplayName => "";

    /// <inheritdoc />
    public string? DirectoryPath => null;

    /// <inheritdoc />
    public bool ImplicitProject => true;
}