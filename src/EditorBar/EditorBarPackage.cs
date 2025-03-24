// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.Services;
using JPSoftworks.EditorBar.Services.LocationProviders;
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
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(PackageGuids.EditorBarString)]
[ProvideOptionPage(typeof(GeneralOptionPage), "Editor Bar", "General", 0, 0, true)]
[ProvideProfile(typeof(GeneralOptionPage), "Editor Bar", "General", 0, 0, true)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[ProvideService(typeof(IMenuContextService), IsAsyncQueryable = true)]
[ProvideService(typeof(ILocationProvider), IsAsyncQueryable = true)]
[ProvideAutoLoad(PackageGuids.EditorBarAutoloadUIContextGuidString, PackageAutoLoadFlags.BackgroundLoad)]
[ProvideUIContextRule(
    // we've to load the package to provide Checked status for the ToggleEditorBarCommand
    PackageGuids.EditorBarAutoloadUIContextGuidString,
    "Auto load when Editor Bar is Enabled",
    "editorBarEnabled",
    ["editorBarEnabled"],
    ["UserSettingsStoreQuery:" + GeneralOptionsModel.PathToEnabledProperty],
    100)]
public sealed class EditorBarPackage : AsyncPackage
{
    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">
    /// Thrown back at the awaiting caller if
    /// <paramref name="cancellationToken" /> is canceled, even if the caller is already on the main thread.
    /// </exception>
    protected override async Task InitializeAsync(
        CancellationToken cancellationToken,
        IProgress<ServiceProgressData> progress)
    {
        try
        {
            this.AddService(typeof(IMenuContextService),
                static (_, _, _) => Task.FromResult<object?>(new MenuContextService()), true);

            this.AddService(typeof(ILocationProvider),
                static (_, _, _) => Task.FromResult<object?>(new ToolkitLocationProvider()), true);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory!.SwitchToMainThreadAsync(cancellationToken);

            // upgrade settings from previous versions
            var options = await GeneralOptionsModel.GetLiveInstanceAsync();

            await options.UpgradeAsync();

            await this.RegisterCommandsAsync();

            RatingService.RegisterSuccessfulUsage();
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
            throw;
        }
    }
}