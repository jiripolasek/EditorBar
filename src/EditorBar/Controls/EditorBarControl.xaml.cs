// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JPSoftworks.EditorBar.Commands;
using JPSoftworks.EditorBar.Commands.Abstractions;
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
    private readonly BarPosition _position;
    private readonly ITextDocument _textDocument;
    private readonly IWpfTextView _textView;
    private readonly EditorBarViewModel _viewModel;

    /// <summary>
    /// Gets a value indicating whether file actions should use the compact overflow surface.
    /// </summary>
    public bool UsesToolbarOverflow => this._position == BarPosition.BottomControl;

    /// <summary>
    /// Gets a value indicating whether file actions should use the standard toolbar surface.
    /// </summary>
    public bool UsesStandardToolbar => !this.UsesToolbarOverflow;

    public EditorBarControl(
        IWpfTextView textView,
        ITextDocument textDocument,
        JoinableTaskFactory joinableTaskFactory,
        IStructureProviderService structureProviderService,
        BarPosition position)
    {
        this._textView = Requires.NotNull(textView);
        this._joinableTaskFactory = Requires.NotNull(joinableTaskFactory);
        this._position = position;
        this._textDocument = Requires.NotNull(textDocument);
        Requires.NotNull(structureProviderService);

        this.DataContext = this._viewModel =
            new EditorBarViewModel(textView, textDocument, joinableTaskFactory, structureProviderService);
        this._viewModel.AddTo(this._disposables);

        this.InitializeComponent();

        this._textView.SetEditorBarControl(this);

        this._delayedSettingsApplicator = new SingleActionGatedExecutor(this.ApplySettings);
        this._delayedSettingsApplicator.RequestExecution();

        var settingsRefreshAggregator = new SettingsRefreshAggregator();
        settingsRefreshAggregator.SettingsRefreshRequested
            += (_, _) => this._delayedSettingsApplicator.RequestExecution();
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

        this.LocateInSolutionExplorerButton!.Visibility = options.ShowLocateInSolutionExplorerButton
            ? Visibility.Visible
            : Visibility.Collapsed;

        this.OpenDefaultEditorButton!.Visibility = options.ShowOpenDefaultEditorButton
            ? Visibility.Visible
            : Visibility.Collapsed;

        this.OpenExternalEditorButton!.Visibility = options.ShowOpenExternalEditorButton &&
                                                    !StringHelper.IsNullOrWhiteSpace(options.ExternalEditorCommand)
            ? Visibility.Visible
            : Visibility.Collapsed;

        this.OpenContainingFolderButton!.Visibility = options.ShowOpenContainingFolderButton
            ? Visibility.Visible
            : Visibility.Collapsed;

        this.OpenTerminalButton!.Visibility = options.ShowOpenTerminalButton
            ? Visibility.Visible
            : Visibility.Collapsed;

        this._viewModel.IsDevelopmentModeEnabled = options.DebugMode;

        this.ReloadThemeResources();

        // Allow the top bar to follow the editor appearance. Bottom positions use the chrome of their actual host.
        switch (this._position)
        {
            case BarPosition.Top:
                switch (GeneralOptionsModel.Instance.VisualStyle)
                {
                    case VisualStyle.FullRowCommandBar:
                        this.Background = (Brush)this.FindResource(VsBrushes.CommandBarGradientKey!)!;
                        this.BorderBrush = (Brush)this.FindResource(SearchControlColors.PopupBorderBrushKey!)!;
                        this.BorderThickness = new Thickness(0, 0, 0, 1);
                        break;
                    case VisualStyle.FullRowTransparent:
                        // Copy the editor background so ImageThemingUtilities can theme images against it.
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

                break;

            case BarPosition.Bottom:
                this.Background = (Brush)this.FindResource(VsBrushes.CommandBarGradientKey!)!;
                this.BorderBrush = Brushes.Transparent;
                this.BorderThickness = new Thickness(0);
                break;

            case BarPosition.BottomControl:
                this.Background = (Brush)this.FindResource(VsBrushes.ScrollBarBackgroundKey!)!;
                this.BorderBrush = Brushes.Transparent;
                this.BorderThickness = new Thickness(0);
                break;

            default:
                throw new ArgumentOutOfRangeException();
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

    private void ApplySettings()
    {
        this._joinableTaskFactory.RunAsync(async () =>
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
        }).FireAndForget();
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

    private void UIElement_OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        // Hotfix: when VS fires View.NavigateBackwards or View.NavigateForwards command from keyboard shortcut,
        // the change of active tab causes VS to change the focus. It moves focus to the first control, which is
        // usually the first button in the editor bar. But the expected behavior is to keep the focus in the editor,
        // so we need to move the focus back to the editor.
        // This event will be raised multiple times, with different sources and old focuses, so I'm
        // going to do the absolutely dumb thing here to make it reliable.

        // Let's assume that we can accept focus changes from buttons that are before or after our button.
        if (e.OldFocus != null && e.OldFocus is not Button)
        {
            this._joinableTaskFactory.RunAsync(async () =>
            {
                await Task.Yield();
                await this._joinableTaskFactory.SwitchToMainThreadAsync();
                this._textView.VisualElement?.Focus();
            }).FireAndForget();
            e.Handled = true;
        }
    }

    private void SettingsMenuButton_OnClick(object sender, RoutedEventArgs e)
    {
        new SettingsMenuContext(this._textView).ShowMenu();
    }

    private void FileActionsMenuButton_OnClick(object sender, RoutedEventArgs e)
    {
        new FileActionMenuContext(this._textDocument).ShowMenu();
    }
}
