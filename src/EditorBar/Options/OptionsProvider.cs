// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Options;

internal partial class OptionsProvider
{
    // Register the options with this attribute on your package class:
    // [ProvideOptionPage(typeof(OptionsProvider.GeneralPageOptions), "EditorBar.Options", "GeneralPage", 0, 0, true, SupportsProfiles = true)]
    [ComVisible(true)]
    public class GeneralPageOptions : BaseOptionPage<GeneralPage> { }
}