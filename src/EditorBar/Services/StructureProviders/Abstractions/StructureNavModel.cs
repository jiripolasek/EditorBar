// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Represents a structure of a document.
/// </summary>
public sealed class StructureNavModel
{
    /// <summary>
    /// Indicates whether a root element can have child elements.
    /// </summary>
    public bool CanRootHaveChildren { get; }

    /// <summary>
    /// Gets the list of structural elements forming the breadcrumb path.
    /// The first element is typically the document root,
    /// and the last element corresponds to the caret's location in the structure.
    /// </summary>
    public IReadOnlyList<BaseStructureModel> Breadcrumbs { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StructureNavModel" /> class.
    /// </summary>
    /// <param name="canRootHaveChildren"></param>
    /// <param name="structure"></param>
    [SetsRequiredMembers]
    public StructureNavModel(bool canRootHaveChildren, IEnumerable<BaseStructureModel> structure)
    {
        this.CanRootHaveChildren = canRootHaveChildren;
        this.Breadcrumbs = structure.ToArray();
    }
}