// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Helpers.Events;
using JPSoftworks.EditorBar.Helpers.Presentation;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.Services;
using JPSoftworks.EditorBar.Services.StructureProviders;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for EditorBarControl.xaml
/// </summary>
internal partial class EditorBarControl : IDisposable
{
    private readonly SingleActionGatedExecutor _delayedSettingsApplicator;
    private readonly CompositeDisposable _disposables = [];
    private readonly JoinableTaskFactory _joinableTaskFactory;
    private readonly IWpfTextView _textView;
    private readonly EditorBarViewModel _viewModel;

    public EditorBarControl(
        IWpfTextView textView,
        ITextDocument textDocument,
        JoinableTaskFactory joinableTaskFactory,
        IStructureProviderService structureProviderService)
    {
        this._textView = Requires.NotNull(textView, nameof(textView));
        this._joinableTaskFactory = Requires.NotNull(joinableTaskFactory, nameof(joinableTaskFactory));
        Requires.NotNull(textDocument, nameof(textDocument));
        Requires.NotNull(structureProviderService, nameof(structureProviderService));

        this.DataContext = this._viewModel =
            new EditorBarViewModel(textView, textDocument, joinableTaskFactory, structureProviderService);

        this.InitializeComponent();

        this._textView.SetEditorBarControl(this);


        this._delayedSettingsApplicator = new SingleActionGatedExecutor(this.ApplySettings);
        this._delayedSettingsApplicator.RequestExecution();

        var settingsRefreshAggregator = new SettingsRefreshAggregator();
        settingsRefreshAggregator.SettingsRefreshRequested += (_, _) => this._delayedSettingsApplicator.RequestExecution();
        settingsRefreshAggregator.AddTo(this._disposables);

        this.IsVisibleChanged += this.OnIsVisibleChanged;
        this.Loaded += (_, _) => this._viewModel.InitializeAsync().FireAndForget();
    }

    public void Dispose()
    {
        this._disposables.Dispose();

        if (ReferenceEquals(this._textView.GetEditorBarControl()!, this))
        {
            this._textView.SetEditorBarControl(null);
        }
    }

    private void ReapplySettings()
    {
        var options = GeneralOptionsModel.Instance;

        this.OpenExternalEditorButton!.Visibility = StringHelper.IsNullOrWhiteSpace(options.ExternalEditorCommand)
            ? Visibility.Collapsed
            : Visibility.Visible;

        this._viewModel.IsDevelopmentModeEnabled = options.DebugMode;

        this.ReloadThemeResources();

        // allow to change background color of the editor bar
        // in case of the top panel, we can follow the color of the editor so it integrates seamlessly;
        // bottom panel is below the scrollbar, so it doesn't make sense to follow the editor color
        if (GeneralOptionsModel.Instance.BarPosition == BarPosition.Top)
        {
            switch (GeneralOptionsModel.Instance.VisualStyle)
            {
                case VisualStyle.FullRowCommandBar:
                    this.Background = (Brush)this.FindResource(VsBrushes.CommandBarGradientKey!)!;
                    this.BorderBrush = (Brush)this.FindResource(SearchControlColors.PopupBorderBrushKey!)!;
                    this.BorderThickness = new Thickness(0, 0, 0, 1);
                    break;
                case VisualStyle.FullRowTransparent:
                    // copy background from the editor so the theme ImageThemingUtilities.ImageBackgroundColor will work
                    this.Background = this._textView.Background ?? Brushes.Transparent;
                    this.BorderBrush = (Brush)this.FindResource(SearchControlColors.PopupBorderBrushKey!)!;
                    this.BorderThickness = new Thickness(0, 0, 0, 1);
                    break;
                case VisualStyle.FullRowToolWindow:
                    this.Background = (Brush)this.FindResource(VsBrushes.ToolWindowBackgroundKey!)!;
                    this.BorderBrush = (Brush)this.FindResource(SearchControlColors.PopupBorderBrushKey!)!;
                    this.BorderThickness = new Thickness(0, 0, 0, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            this.Background = (Brush)this.FindResource(VsBrushes.CommandBarGradientKey!)!;
            this.BorderBrush = Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
        }
    }

    private void ReloadThemeResources()
    {
        StyleHelper.ReplaceResourceDictionary(
            this.Resources.MergedDictionaries,
            "/Themes/EditorBar.",
            $"{GeneralOptionsModel.Instance.DisplayStyle}.xaml");
        this.ForceReloadResources();
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var isVisible = (bool)e.NewValue;
        if (isVisible)
        {
            this._viewModel.ResumeAsync().FireAndForget();
            this._delayedSettingsApplicator.OpenGate();
        }
        else
        {
            this._viewModel.Suspend();
            this._delayedSettingsApplicator.CloseGate();
        }
    }

    [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "<Pending>")]
    private async void ApplySettings()
    {
        try
        {
            await this._joinableTaskFactory.SwitchToMainThreadAsync();
            this.ReapplySettings();
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }
    }

    public void FocusAndOpenProjectCrumb()
    {
        this.EditorBarBreadcrumbs!.FocusFirstBreadcrumbOfType<ProjectContainerBreadcrumbModel>();
    }

    public void FocusAndOpenFirstSymbolCrumb()
    {
        this.EditorBarBreadcrumbs!
            .FocusFirstBreadcrumbOfType<StructureBreadcrumbViewModel>(static t => t.CanHaveChildren);
    }

    public void FocusAndOpenLastSymbolCrumb()
    {
        this.EditorBarBreadcrumbs!
            .FocusLastBreadcrumbOfType<StructureBreadcrumbViewModel>(static t => t.CanHaveChildren);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            this._textView.VisualElement?.Focus();
        }
    }
}