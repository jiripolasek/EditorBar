// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Threading;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar;

/// <summary>
/// This is the class that implements the package exposed by this assembly.
/// </summary>
/// <remarks>
/// <para>
/// The minimum requirement for a class to be considered a valid package for Visual Studio
/// is to implement the IVsPackage interface and register itself with the shell.
/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
/// to do it: it derives from the Package class that provides the implementation of the
/// IVsPackage interface and uses the registration attributes defined in the framework to
/// register itself and its components with the shell. These attributes tell the pkgdef creation
/// utility what data to put into .pkgdef file.
/// </para>
/// <para>
/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
/// &gt; in .vsixmanifest file.
/// </para>
/// </remarks>
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(PackageGuidString)]
[ProvideOptionPage(typeof(OptionsProvider.GeneralPageOptions), "Editor Bar", "General", 0, 0, true)]
[ProvideProfile(typeof(OptionsProvider.GeneralPageOptions), "Editor Bar", "General", 0, 0, true)]
public sealed class EditorBarPackage : AsyncPackage
{
    /// <summary>
    /// EditorBarPackage GUID string.
    /// </summary>
    private const string PackageGuidString = "ef5d9a25-5e0d-4428-8762-56d4dc816eeb";


    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">Thrown back at the awaiting caller if <paramref name="cancellationToken" /> is canceled, even if the caller is already on the main thread.</exception>
    protected override async Task InitializeAsync(CancellationToken cancellationToken,
        IProgress<ServiceProgressData> progress)
    {
        // When initialized asynchronously, the current thread may be a background thread at this point.
        // Do any initialization that requires the UI thread after switching to the UI thread.
        await this.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);
    }
}