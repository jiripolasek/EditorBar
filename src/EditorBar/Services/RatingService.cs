// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Services;

internal static class RatingService
{
    /// <summary>
    /// Defines number of time the extension is used before the rating prompt is shown (usage is incremented on successful
    /// usage, only once per session).
    /// </summary>
    private const int UsagesBeforeRatingPrompt = 6;

    public static void RegisterSuccessfulUsage()
    {
        var prompt = new RatingPrompt("jiripolasek.EditorBar", Vsix.Name, GeneralOptionsModel.Instance,
            UsagesBeforeRatingPrompt);
        prompt.RegisterSuccessfulUsage();
    }
}