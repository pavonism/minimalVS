using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

namespace MinimalVS
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionPage), "Environment", "MinimalVS", 0, 0, true, new[] { "MinimalVS", "menu", "toolbar", "title", "hide" })]
    public sealed class MinimalVSPackage : AsyncPackage
    {
        public const string PackageGuidString = "a1f9205f-bdb8-4707-90af-bd6f484fbc0f";

        #region Package Members
        private Window _mainWindow;

        private OptionPage _options;
        public OptionPage Options
        {
            get
            {
                if (_options == null)
                {
                    _options = (OptionPage)GetDialogPage(typeof(OptionPage));
                }
                return _options;
            }
        }

        private bool _isMenuVisible;
        public bool IsMenuVisible
        {
            get
            {
                return _isMenuVisible;
            }
            set
            {
                if (_isMenuVisible == value)
                    return;
                _isMenuVisible = value;

                if(Options.HideVariant != HideVariant.HideOnlyToolbar)
                {
                    UpdateMenuHeight();
                    UpdateTitleHeight();
                }

                if(Options.HideVariant != HideVariant.HideOnlyMenu)
                {
                    UpdateToolbarHeight();
                }
            }
        }

        private FrameworkElement _titleBar;
        public FrameworkElement TitleBar
        {
            get
            {
                return _titleBar;
            }
            set
            {
                _titleBar = value;
                UpdateTitleHeight();
            }
        }

        private FrameworkElement _menuBar;
        public FrameworkElement MenuBar
        {
            get
            {
                return _menuBar;
            }
            set
            {
                _menuBar = value;
                UpdateMenuHeight();
            }
        }

        private FrameworkElement _feedbackPanel;
        public FrameworkElement FeedbackPanel
        {
            get
            {
                return _feedbackPanel;
            }
            set
            {
                _feedbackPanel = value;
                UpdateFeedbackVisibility();
            }
        }

        private FrameworkElement _toolbar;
        public FrameworkElement Toolbar
        {
            get
            {
                return _toolbar;
            }
            set
            {
                _toolbar = value;
                UpdateToolbarHeight();
            }
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await MenuVisibilityToogleCommand.InitializeAsync(this);

            _mainWindow = Application.Current.MainWindow;
            if (_mainWindow == null)
            {
                Trace.TraceError("mainWindow is null");
                return;
            }

            _mainWindow.LayoutUpdated += DetectLayoutElements;
            Options.PropertyChanged += OptionsChanged;
        }

        public void SwitchMenuVisibility()
        {
            IsMenuVisible = !IsMenuVisible;
        }

        private void OptionsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Options.HideVariant):
                    UpdateMenuHeight();
                    UpdateTitleHeight();
                    UpdateToolbarHeight();
                    break;
                case nameof(Options.HideFeedback):
                    UpdateFeedbackVisibility();
                    break;
            }
        }

        void UpdateTitleHeight()
        {
            if (_titleBar == null)
            {
                return;
            }
            if (!IsMenuVisible && Options.HideVariant != HideVariant.HideOnlyToolbar)
            {
                _titleBar.Height = 0;
            }
            else
            {
                _titleBar.ClearValue(FrameworkElement.HeightProperty);
            }
        }

        private void UpdateMenuHeight()
        {
            if (_menuBar == null)
            {
                return;
            }
            if (!IsMenuVisible && Options.HideVariant != HideVariant.HideOnlyToolbar)
            {
                _menuBar.Height = 0;
            }
            else
            {
                _menuBar.ClearValue(FrameworkElement.HeightProperty);
            }
        }

        private void UpdateFeedbackVisibility()
        {
            if (_feedbackPanel != null)
            {
                _feedbackPanel.Visibility = Options.HideFeedback ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void UpdateToolbarHeight()
        {
            if (_toolbar == null)
            {
                return;
            }
            if (!IsMenuVisible && Options.HideVariant != HideVariant.HideOnlyMenu)
            {
                _toolbar.MaxHeight = 0;
                _toolbar.Margin = new Thickness(0);
            }
            else
            {
                _toolbar.ClearValue(FrameworkElement.MaxHeightProperty);
                _toolbar.Margin = new Thickness(0, 0, 0, 4);
            }
        }

        private void DetectLayoutElements(object sender, EventArgs e)
        {
            if (MenuBar == null)
            {
                foreach (var descendant in _mainWindow.FindDescendants<Menu>())
                {
                    if (AutomationProperties.GetAutomationId(descendant) == "MenuBar")
                    {
                        FrameworkElement frameworkElement = descendant;
                        var parent = descendant.GetVisualOrLogicalParent();
                        if (parent != null)
                            frameworkElement = parent.GetVisualOrLogicalParent() as DockPanel ?? frameworkElement;
                        MenuBar = frameworkElement;
                        break;
                    }
                }
            }
            if (TitleBar == null)
            {
                var titleBar = _mainWindow.FindDescendants<MainWindowTitleBar>().FirstOrDefault();
                if (titleBar != null)
                {
                    TitleBar = titleBar;
                }
            }
            if (FeedbackPanel == null)
            {
                FeedbackPanel = Application.Current.MainWindow.FindElement("FrameControlContainerBorder");
            }
            if (TitleBar != null && Toolbar == null)
            {
                Toolbar = Application.Current.MainWindow.FindElement("TopDockBorder");
            }
            if (Toolbar != null && MenuBar != null)
            {
                _mainWindow.LayoutUpdated -= DetectLayoutElements;
            }
        }
        #endregion
    }
}
