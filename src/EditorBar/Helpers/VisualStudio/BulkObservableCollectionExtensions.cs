// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Clears a bulk observable collection and adds a new range of items. It ensures the operation is treated as a single
/// bulk action.
/// </summary>
internal static class BulkObservableCollectionExtensions
{
    /// <summary>
    /// Sets a range of items in a bulk observable collection by clearing existing items and adding new ones.
    /// </summary>
    /// <typeparam name="T">Represents the type of elements that the collection will hold.</typeparam>
    /// <param name="bulkCollection">The collection that will be modified to include a new range of items.</param>
    /// <param name="collection">The new set of items to be added to the collection after clearing it.</param>
    internal static void SetRange<T>(this BulkObservableCollection<T> bulkCollection, IEnumerable<T>? collection)
    {
        Requires.NotNull(bulkCollection, nameof(bulkCollection));

        bulkCollection.BeginBulkOperation();
        bulkCollection.Clear();
        bulkCollection.AddRange(collection!);
        bulkCollection.EndBulkOperation();
    }
}