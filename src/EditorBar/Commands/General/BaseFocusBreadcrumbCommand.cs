// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Controls;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JPSoftworks.EditorBar.Commands;

internal abstract class BaseFocusBreadcrumbCommand<T> : BaseCommand<T> where T : class, new()
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        // Always switch to the main thread when dealing with Visual Studio services
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        // 1) Get the active document view
        var docView = await VS.Documents.GetActiveDocumentViewAsync();

        // 2) Retrieve the IWpfTextViewHost from IVsUserData
        var vsTextView = docView?.TextView?.Properties!.GetProperty<IVsTextView>(typeof(IVsTextView));
        if (vsTextView == null)
        {
            return;
        }

        // get host
        var componentModel = await this.Package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
        var vsEditorAdaptersFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
        var wpfTextViewHost = vsEditorAdaptersFactoryService.GetWpfTextViewHost(vsTextView);

        // 3) Get your custom margin by name
        //    This name must match what you used in [Name("MyCustomMarginName")] in your IWpfTextViewMarginProvider
        var editorBarControl = FindVisibleOne(wpfTextViewHost);
        if (editorBarControl != null)
        {
            // 4) Invoke a method on your margin
            this.ExecuteCore(editorBarControl);
        }
    }

    protected abstract void ExecuteCore(EditorBarControl control);

    private static EditorBarControl? FindVisibleOne(IWpfTextViewHost? wpfTextViewHost)
    {
        if (wpfTextViewHost.TextView.Properties.TryGetProperty(typeof(EditorBarControl),
                out EditorBarControl editorBarControl))
        {
            return editorBarControl;
        }

        return null;
    }
}