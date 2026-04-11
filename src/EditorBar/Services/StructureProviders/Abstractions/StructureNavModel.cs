// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Represents a structure of a document.
/// </summary>
public sealed class StructureNavModel
{
    /// <summary>
    /// Gets a value indicating whether a root element can have child elements.
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
    /// <param name="canRootHaveChildren">Whether a root element can have child elements.</param>
    /// <param name="structure">The structural elements forming the breadcrumb path.</param>
    public StructureNavModel(bool canRootHaveChildren, IEnumerable<BaseStructureModel> structure)
    {
        this.CanRootHaveChildren = canRootHaveChildren;
        this.Breadcrumbs = structure.ToArray();
    }
}
