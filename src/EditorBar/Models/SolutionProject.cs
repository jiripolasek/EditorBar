// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

namespace JPSoftworks.EditorBar.Models;

public abstract class SolutionProject : IProjectWrapper
{
    public string DisplayName { get; protected init; }

    public string? DirectoryPath { get; protected init; }
}