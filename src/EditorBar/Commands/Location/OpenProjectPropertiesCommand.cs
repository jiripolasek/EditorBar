// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Services.LocationProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarProjectBreadcrumbMenu_OpenPropertiesCommand)]
internal sealed class
    OpenProjectPropertiesCommand : BaseLocationMenuContextCommand<OpenProjectPropertiesCommand>
{
    protected override Task ExecuteCoreAsync(IProjectInfo project, IWpfTextView wpfTextView)
    {
        return Task.CompletedTask;

        //if (projectWrapper.Project.IsLoaded)
        //{
        //    var vsProject3 = await GetIVsProject3Async(projectWrapper.Project);
        //    if (vsProject3 != null)
        //    {
        //        try
        //        {

        //            IVsUIShellOpenDocument uiShellOpenDocument = (IVsUIShellOpenDocument)await ServiceProvider.GetGlobalServiceAsync(typeof(SVsUIShellOpenDocument));
        //            Guid logicalView = VSConstants.LOGVIEWID_Primary;
        //            string propertyPages = "PropertyPages";
        //            IVsWindowFrame frame;
        //            Guid projectGuid = Guid.Empty;

        //            uiShellOpenDocument.OpenSpecificEditor(
        //                0,
        //            propertyPages,
        //                ref projectGuid,
        //                null,
        //                ref logicalView,
        //            "",
        //                ref projectGuid,
        //                IntPtr.Zero,
        //                out frame);

        //            if (frame != null)
        //            {
        //                frame.Show();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await VS.MessageBox.ShowErrorAsync("Error", $"Could not open project properties: {ex.Message}");
        //        }
        //    }
        //}
    }
}