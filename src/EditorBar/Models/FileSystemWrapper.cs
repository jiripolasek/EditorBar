// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

namespace JPSoftworks.EditorBar.Models;

public class FileSystemWrapper : IProjectWrapper
{
    public string DisplayName { get; }

    public string? DirectoryPath { get; }

    public FileSystemWrapper(string displayName, string? path)
    {
        this.DisplayName = displayName;
        this.DirectoryPath = path;
    }
}