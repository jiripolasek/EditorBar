// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Options;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Used by attribute on EditorBarPackage")]
internal class OptionsProvider
{
    [ComVisible(true)]
    public class GeneralPageOptions : BaseOptionPage<GeneralPage>;
}