// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

namespace JPSoftworks.EditorBar.Models;

public interface IProjectWrapper
{
    public string DisplayName { get; }

    public string? DirectoryPath { get; }
}