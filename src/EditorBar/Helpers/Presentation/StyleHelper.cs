// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reflection;
using System.Windows;
using Microsoft;

namespace JPSoftworks.EditorBar.Helpers.Presentation;

/// <summary>
/// Helper methods for working with styles in WPF applications.
/// </summary>
public static class StyleHelper
{
    /// <summary>
    /// Reloads a style in the specified resource collection based on the provided XAML file pattern and naming convention.
    /// If the desired style is already present and is the only matching style, no action is taken.
    /// </summary>
    public static void ReplaceResourceDictionary(
        ICollection<ResourceDictionary> resourceCollection,
        string xamlFilePattern,
        string newStyleKey)
    {
        Requires.NotNull(resourceCollection, nameof(resourceCollection));
        Requires.NotNullOrWhiteSpace(xamlFilePattern, nameof(xamlFilePattern));
        Requires.NotNullOrWhiteSpace(newStyleKey, nameof(newStyleKey));

        // Infer the calling assembly's pack URI base
        var callingAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
        var baseUri = $"pack://application:,,,/{callingAssemblyName};component";

        ReplaceResourceDictionaryInternal(resourceCollection, baseUri, xamlFilePattern, newStyleKey);
    }

    /// <summary>
    /// Force WPF framework to reload styles on the specified element.
    /// </summary>
    public static void ForceReloadResources(this FrameworkElement element)
    {
        Requires.NotNull(element, nameof(element));

        var currentResources = element.Resources;
        element.Resources = null!;
        element.Resources = currentResources;
        element.InvalidateVisual();
        element.UpdateLayout();
    }

    /// <summary>
    /// Internal method to handle the logic for reloading styles.
    /// </summary>
    private static void ReplaceResourceDictionaryInternal(
        ICollection<ResourceDictionary> resourceCollection,
        string baseUri,
        string xamlFilePattern,
        string newStyleKey)
    {
        // Remove old styles matching the specified pattern
        var matchingDictionaries = resourceCollection
            .Where(resource => IsMatchingStyle(resource, baseUri + xamlFilePattern))
            .ToList();

        var newStyleUriKey = $"{baseUri}{xamlFilePattern}{newStyleKey}";

        if (matchingDictionaries.Count == 1 &&
            matchingDictionaries[0]?.Source != null &&
            string.Equals(matchingDictionaries[0]!.Source!.ToString(), newStyleUriKey,
                StringComparison.OrdinalIgnoreCase))
        {
            // Desired style is already applied, no action needed
            return;
        }


        foreach (var resourceDictionary in matchingDictionaries)
        {
            resourceCollection.Remove(resourceDictionary);
        }

        // Add new style
        resourceCollection.Add(new ResourceDictionary { Source = new Uri(newStyleUriKey, UriKind.Absolute) });

        return;

        // Local helper method to determine if a resource matches the specified pattern
        static bool IsMatchingStyle(ResourceDictionary resourceDictionary, string pattern)
        {
            return resourceDictionary.Source != null &&
                   resourceDictionary.Source.IsAbsoluteUri &&
                   resourceDictionary.Source.AbsoluteUri.Contains(pattern);
        }
    }
}