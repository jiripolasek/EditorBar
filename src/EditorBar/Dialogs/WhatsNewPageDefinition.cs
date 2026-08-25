// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Dialogs.WhatsNewPages;

namespace JPSoftworks.EditorBar.Dialogs;

[Flags]
internal enum WhatsNewPageAudience
{
    None = 0,
    FirstRun = 1,
    Upgrade = 2,
    Manual = 4,
    All = FirstRun | Upgrade | Manual
}

internal sealed class WhatsNewPageDefinition
{
    internal WhatsNewPageDefinition(
        WhatsNewPageControl content,
        Version introducedInVersion,
        WhatsNewPageAudience audiences)
    {
        this.Content = content;
        this.Heading = content.Heading;
        this.Subheading = content.Subheading;
        this.IntroducedInVersion = introducedInVersion;
        this.Audiences = audiences;
    }

    internal WhatsNewPageControl Content { get; }

    internal string Heading { get; }

    internal string Subheading { get; }

    internal Version IntroducedInVersion { get; }

    internal WhatsNewPageAudience Audiences { get; }

    internal bool ShouldShow(WhatsNewDisplayContext context)
    {
        if (this.IntroducedInVersion.CompareTo(context.CurrentVersion) > 0)
        {
            return false;
        }

        var audience = !context.IsAutomatic
            ? WhatsNewPageAudience.Manual
            : context.IsFirstRun
                ? WhatsNewPageAudience.FirstRun
                : WhatsNewPageAudience.Upgrade;

        if ((this.Audiences & audience) == 0)
        {
            return false;
        }

        return audience != WhatsNewPageAudience.Upgrade ||
               context.PreviouslySeenVersion == null ||
               this.IntroducedInVersion.CompareTo(context.PreviouslySeenVersion) > 0;
    }
}
