using System.Runtime.InteropServices;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Lumenhop.Pages;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using Windows.UI;
using WinRT;

namespace Lumenhop;

public sealed partial class MainWindow : Window
{
    public const int FlyoutWidth = 400;
    public const int FlyoutHeight = 600;

    private static readonly string IconPath = Path.Combine(
        AppContext.BaseDirectory,
        "Assets",
        "Lumenhop.ico"
    );

    private DesktopAcrylicController? _glass;
    private SystemBackdropConfiguration? _backdropConfig;
    private DispatcherQueueHelper? _dispatcherHelper;
    private TaskbarIcon? _tray;
    private MenuFlyoutItem? _trayOpen;
    private MenuFlyoutItem? _trayClose;
    private bool _allowClose;
    private bool _acrylicOn = true;
    private int _opacityPct = 28;

    private static readonly TimeSpan UpdateCheckInterval = TimeSpan.FromHours(6);
    private DispatcherQueueTimer? _updateTimer;
    private UpdateOffer? _pendingOffer;
    private string? _notifiedVersion;

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        if (File.Exists(IconPath))
            AppWindow.SetIcon(IconPath);

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsResizable = false;
        }

        TrySetGlassBackdrop();
        AppWindow.Closing += OnWindowClosing;
        AppWindow.Changed += OnWindowChanged;
        SetupTray();
        PingMonitor.Instance.Start(DispatcherQueue);
        ApplyAnchor();
        StartUpdateTimer();

        NavView.Loaded += (_, _) => NavView.SelectedItem = HomeNav;
    }

    public ElementTheme RootTheme => RootGrid.ActualTheme;

    public void ApplyTheme(string theme)
    {
        RootGrid.RequestedTheme = theme switch
        {
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
    }

    public void ReloadForLanguage(string language)
    {
        App.ApplyCulture(language);
        Loc.Reset();
        HomeNav.Content = Loc.Get("Nav_Home/Content");
        OutageNav.Content = Loc.Get("Nav_Outage/Content");
        AboutNav.Content = Loc.Get("Nav_About/Content");
        var current = ContentFrame.CurrentSourcePageType;
        if (current is not null)
        {
            ContentFrame.Navigate(current);
            ContentFrame.BackStack.Clear();
        }

        ApplyTrayLanguage();
    }

    public void ExitApplication()
    {
        _allowClose = true;
        _updateTimer?.Stop();
        PingMonitor.Instance.Stop();
        _tray?.Dispose();
        Close();
    }

    public void ShowFromTray()
    {
        ApplyAnchor();
        AppWindow.Show();
        SetForegroundWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));
    }

    public void ApplyAnchor()
    {
        AppWindow.Resize(new SizeInt32(FlyoutWidth, FlyoutHeight));
        var work = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest).WorkArea;
        var anchor = WindowPlacement.Parse(SettingsStore.Load().WindowAnchor);
        var (x, y) = WindowPlacement.Compute(
            work.X,
            work.Y,
            work.Width,
            work.Height,
            FlyoutWidth,
            FlyoutHeight,
            anchor
        );
        AppWindow.Move(new PointInt32(x, y));
    }

    public void ShowUpdate(UpdateOffer offer)
    {
        ShowFromTray();
        OpenUpdateWindow(offer);
    }

    /// <summary>Announces a newer build with a discreet tray toast, never a pop-up window.</summary>
    public void NotifyUpdate(UpdateOffer offer)
    {
        _pendingOffer = offer;
        if (_notifiedVersion == offer.Version)
            return;

        _notifiedVersion = offer.Version;
        _tray?.ShowNotification(
            Loc.Get("Update_ToastTitle"),
            string.Format(Loc.Get("Update_ToastBody"), offer.Version),
            NotificationIcon.Info
        );
    }

    private void OpenFromTray()
    {
        ShowFromTray();
        if (_pendingOffer is not null)
            OpenUpdateWindow(_pendingOffer);
    }

    private static void OpenUpdateWindow(UpdateOffer offer)
    {
        var window = new UpdateWindow(offer);
        window.Activate();
    }

    private void StartUpdateTimer()
    {
        _updateTimer = DispatcherQueue.CreateTimer();
        _updateTimer.Interval = UpdateCheckInterval;
        _updateTimer.IsRepeating = true;
        _updateTimer.Tick += (_, _) => _ = CheckForUpdatesAsync();
        _updateTimer.Start();
    }

    private async Task CheckForUpdatesAsync()
    {
        if (!SettingsStore.Load().CheckUpdatesOnLaunch)
            return;

        var check = await UpdateService.CheckAsync();
        if (check.Kind == UpdateCheckKind.Available && check.Offer is not null)
            NotifyUpdate(check.Offer);
    }

    public void ApplyBackdropSettings()
    {
        var settings = SettingsStore.Load();
        _acrylicOn = settings.AcrylicEnabled;
        _opacityPct = Math.Clamp(settings.BackgroundOpacity, 0, 100);
        ApplyGlassColors();
    }

    private void OnNavSelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args
    )
    {
        Type page = typeof(HomePage);
        if (args.IsSettingsSelected)
            page = typeof(SettingsPage);
        else if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
            page = tag switch
            {
                "outage" => typeof(OutagePage),
                "about" => typeof(AboutPage),
                _ => typeof(HomePage),
            };

        if (ContentFrame.CurrentSourcePageType != page)
            ContentFrame.Navigate(page);
    }

    private void TrySetGlassBackdrop()
    {
        if (!DesktopAcrylicController.IsSupported())
            return;

        _dispatcherHelper = new DispatcherQueueHelper();
        _dispatcherHelper.EnsureDispatcherQueueController();
        _backdropConfig = new SystemBackdropConfiguration { IsInputActive = true };
        Activated += (_, e) =>
            _backdropConfig.IsInputActive =
                e.WindowActivationState != WindowActivationState.Deactivated;
        Closed += (_, _) =>
        {
            _glass?.Dispose();
            _glass = null;
        };
        RootGrid.ActualThemeChanged += (_, _) => UpdateGlassTheme();
        UpdateGlassTheme();
        _glass = new DesktopAcrylicController();
        ApplyBackdropSettings();
        _glass.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
        _glass.SetSystemBackdropConfiguration(_backdropConfig);
    }

    private void UpdateGlassTheme()
    {
        if (_backdropConfig is null)
            return;
        _backdropConfig.Theme = RootGrid.ActualTheme switch
        {
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            ElementTheme.Light => SystemBackdropTheme.Light,
            _ => SystemBackdropTheme.Default,
        };
        ApplyGlassColors();
    }

    private void ApplyGlassColors()
    {
        var dark = RootGrid.ActualTheme != ElementTheme.Light;
        var baseColor = dark ? Color.FromArgb(255, 12, 18, 24) : Color.FromArgb(255, 244, 247, 250);

        if (_glass is not null)
        {
            _glass.TintColor = baseColor;
            _glass.FallbackColor = dark
                ? Color.FromArgb(255, 22, 30, 38)
                : Color.FromArgb(255, 244, 247, 250);
            _glass.TintOpacity = 0.0f;
            _glass.LuminosityOpacity = 0.12f;
        }

        var alpha =
            _acrylicOn && _glass is not null
                ? (byte)Math.Clamp(_opacityPct * 255 / 100, 0, 255)
                : (byte)255;
        RootGrid.Background = new SolidColorBrush(
            Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B)
        );
    }

    private void SetupTray()
    {
        var show = new RelayCommand(OpenFromTray);
        var close = new RelayCommand(ExitApplication);

        _trayOpen = new MenuFlyoutItem { Command = show };
        _trayClose = new MenuFlyoutItem { Command = close };
        ApplyTrayLanguage();

        var menu = new MenuFlyout();
        menu.Items.Add(_trayOpen);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(_trayClose);

        _tray = new TaskbarIcon
        {
            ToolTipText = "Lumenhop",
            LeftClickCommand = show,
            DoubleClickCommand = show,
            MenuActivation = PopupActivationMode.RightClick,
            ContextMenuMode = ContextMenuMode.PopupMenu,
            ContextFlyout = menu,
            IconSource = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                new Uri("ms-appx:///Assets/Lumenhop.ico")
            ),
        };
        _tray.ForceCreate();
        TrySetTrayFileIcon();
    }

    private void ApplyTrayLanguage()
    {
        if (_trayOpen is not null)
            _trayOpen.Text = Loc.Get("Tray_Open");
        if (_trayClose is not null)
            _trayClose.Text = Loc.Get("Tray_Close");
    }

    private void TrySetTrayFileIcon()
    {
        try
        {
            if (File.Exists(IconPath))
                _tray!.Icon = new System.Drawing.Icon(IconPath, 32, 32);
        }
        catch { }
    }

    private void OnWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_allowClose)
            return;
        args.Cancel = true;
        HideToTray();
    }

    private void OnWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (
            AppWindow.Presenter is OverlappedPresenter p
            && p.State == OverlappedPresenterState.Minimized
        )
            HideToTray();
    }

    private void HideToTray()
    {
        if (
            AppWindow.Presenter is OverlappedPresenter p
            && p.State == OverlappedPresenterState.Minimized
        )
            p.Restore();
        AppWindow.Hide();
    }

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
}
