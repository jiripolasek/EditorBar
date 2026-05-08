// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides methods to query and toggle the Visual Studio navigation bar (dropdown bar)
/// for the language associated with the current text view.
/// </summary>
internal static class NavigationBarHelper
{
    /// <summary>
    /// Gets a value indicating whether the navigation bar is currently shown for the language
    /// of the given text view.
    /// </summary>
    /// <param name="textView">The text view.</param>
    /// <returns><c>true</c> if the navigation bar is enabled; <c>false</c> otherwise; <c>null</c> if not determinable.</returns>
    internal static bool? IsNavigationBarEnabled(ITextView textView)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (textView?.TextBuffer?.ContentType == null)
        {
            return null;
        }

        var textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager2;
        if (textManager == null)
        {
            return null;
        }

        var langPrefs = GetLangPrefsForContentType(textManager, textView);
        if (langPrefs == null)
        {
            return null;
        }

        return langPrefs[0].fDropdownBar != 0;
    }

    /// <summary>
    /// Toggles the navigation bar (dropdown bar) visibility for the language associated with the given text view.
    /// </summary>
    /// <param name="textView">The text view.</param>
    /// <returns><c>true</c> if the navigation bar is now enabled after toggling; <c>false</c> if now disabled; <c>null</c> if the operation failed.</returns>
    internal static bool? ToggleNavigationBar(ITextView textView)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (textView?.TextBuffer?.ContentType == null)
        {
            return null;
        }

        var textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager2;
        if (textManager == null)
        {
            return null;
        }

        var langPrefs = GetLangPrefsForContentType(textManager, textView);
        if (langPrefs == null)
        {
            return null;
        }

        langPrefs[0].fDropdownBar = langPrefs[0].fDropdownBar != 0 ? 0u : 1u;

        var hr = textManager.SetUserPreferences2(null, null, langPrefs, null);
        if (ErrorHandler.Failed(hr))
        {
            return null;
        }

        return langPrefs[0].fDropdownBar != 0;
    }

    private static LANGPREFERENCES2[]? GetLangPrefsForContentType(IVsTextManager2 textManager, ITextView textView)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var langGuid = GetLanguageServiceGuid(textView);
        if (langGuid == Guid.Empty)
        {
            return null;
        }

        var langPrefs = new LANGPREFERENCES2[1];
        langPrefs[0].guidLang = langGuid;

        var hr = textManager.GetUserPreferences2(null, null, langPrefs, null);
        if (ErrorHandler.Failed(hr))
        {
            return null;
        }

        return langPrefs;
    }

    private static Guid GetLanguageServiceGuid(ITextView textView)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        // Try to get the IVsTextBuffer and query the language service ID
        if (textView.TextBuffer?.Properties != null &&
            textView.TextBuffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer vsTextBuffer))
        {
            if (vsTextBuffer != null)
            {
                var hr = vsTextBuffer.GetLanguageServiceID(out var langGuid);
                if (ErrorHandler.Succeeded(hr) && langGuid != Guid.Empty)
                {
                    return langGuid;
                }
            }
        }

        // Fallback: try from the document buffer (for projection/embedded scenarios)
        if (textView.TextDataModel?.DocumentBuffer?.Properties != null &&
            textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer docVsTextBuffer))
        {
            if (docVsTextBuffer != null)
            {
                var hr = docVsTextBuffer.GetLanguageServiceID(out var langGuid);
                if (ErrorHandler.Succeeded(hr) && langGuid != Guid.Empty)
                {
                    return langGuid;
                }
            }
        }

        return Guid.Empty;
    }
}
