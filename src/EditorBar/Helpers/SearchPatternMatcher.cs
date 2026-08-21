// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Text;
using System.Text.RegularExpressions;

namespace JPSoftworks.EditorBar.Helpers;

internal sealed class SearchPatternMatcher
{
    private readonly string _filter;
    private readonly Regex? _wildcardRegex;

    public SearchPatternMatcher(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            throw new ArgumentException("Filter must not be empty.", nameof(filter));
        }

        this._filter = filter;
        this._wildcardRegex = ContainsWildcard(filter)
            ? CreateWildcardRegex(filter)
            : null;
    }

    public bool IsMatch(string? searchText)
    {
        var candidate = searchText ?? string.Empty;
        return this._wildcardRegex?.IsMatch(candidate) ??
               candidate.IndexOf(this._filter, StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    private static bool ContainsWildcard(string filter)
    {
        return filter.IndexOfAny(['*', '?']) >= 0;
    }

    private static Regex CreateWildcardRegex(string filter)
    {
        var patternBuilder = new StringBuilder(filter.Length * 2);
        foreach (var character in filter)
        {
            switch (character)
            {
                case '*':
                    patternBuilder.Append(".*");
                    break;
                case '?':
                    patternBuilder.Append('.');
                    break;
                default:
                    patternBuilder.Append(Regex.Escape(character.ToString()));
                    break;
            }
        }

        return new Regex(patternBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }
}
