using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Lumenhop.Pages;

public sealed partial class SettingsPage : Page
{
    private bool _loading = true;

    public SettingsPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var settings = SettingsStore.Load();
        FillPolling(settings.DefaultPollingSeconds);
        FillLanguage(settings.Language);
        FillTheme(settings.Theme);
        StartupSwitch.IsOn = settings.StartWithWindows;
        AcrylicSwitch.IsOn = settings.AcrylicEnabled;
        OpacitySlider.Value = settings.BackgroundOpacity;
        OpacityValue.Text = $"{settings.BackgroundOpacity}%";
        OpacitySlider.IsEnabled = settings.AcrylicEnabled;
        UpdatesSwitch.IsOn = settings.CheckUpdatesOnLaunch;
        CornerPicker.Anchor = WindowPlacement.Parse(settings.WindowAnchor);
        _loading = false;
    }

    private void OnAnchorChanged(object? sender, WindowAnchor anchor)
    {
        if (_loading)
            return;
        var settings = SettingsStore.Load();
        settings.WindowAnchor = anchor.ToString();
        SettingsStore.Save(settings);
        App.Main?.ApplyAnchor();
    }

    private void FillPolling(int selected)
    {
        PollingBox.Items.Clear();
        foreach (var seconds in PollingOptions.Seconds)
            AddItem(PollingBox, string.Format(Loc.Get("Editor_PollingItem"), seconds), seconds);
        SelectTag(PollingBox, PollingOptions.Clamp(selected));
    }

    private void FillLanguage(string selected)
    {
        LanguageBox.Items.Clear();
        AddItem(LanguageBox, Loc.Get("Settings_LanguageSystem"), AppLanguage.System);
        AddItem(LanguageBox, "Português (Brasil)", AppLanguage.Portuguese);
        AddItem(LanguageBox, "English", AppLanguage.English);
        SelectTag(LanguageBox, string.IsNullOrWhiteSpace(selected) ? AppLanguage.System : selected);
    }

    private void FillTheme(string selected)
    {
        ThemeBox.Items.Clear();
        AddItem(ThemeBox, Loc.Get("Settings_ThemeSystem"), "System");
        AddItem(ThemeBox, Loc.Get("Settings_ThemeDark"), "Dark");
        AddItem(ThemeBox, Loc.Get("Settings_ThemeLight"), "Light");
        SelectTag(ThemeBox, selected);
    }

    private static void AddItem(ComboBox box, string label, object tag) =>
        box.Items.Add(new ComboBoxItem { Content = label, Tag = tag });

    private static void SelectTag(ComboBox box, object tag)
    {
        foreach (ComboBoxItem item in box.Items)
        {
            if (Equals(item.Tag, tag))
            {
                box.SelectedItem = item;
                return;
            }
        }
        if (box.Items.Count > 0)
            box.SelectedIndex = 0;
    }

    private void OnPollingChanged(object sender, SelectionChangedEventArgs e)
    {
        if (
            _loading
            || PollingBox.SelectedItem is not ComboBoxItem item
            || item.Tag is not int seconds
        )
            return;
        var settings = SettingsStore.Load();
        settings.DefaultPollingSeconds = seconds;
        SettingsStore.Save(settings);
    }

    private void OnLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        if (
            _loading
            || LanguageBox.SelectedItem is not ComboBoxItem item
            || item.Tag is not string language
        )
            return;
        var settings = SettingsStore.Load();
        settings.Language = language;
        SettingsStore.Save(settings);
        App.Main?.ReloadForLanguage(language);
    }

    private void OnThemeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (
            _loading
            || ThemeBox.SelectedItem is not ComboBoxItem item
            || item.Tag is not string theme
        )
            return;
        var settings = SettingsStore.Load();
        settings.Theme = theme;
        SettingsStore.Save(settings);
        App.Main?.ApplyTheme(theme);
    }

    private void OnAcrylicToggled(object sender, RoutedEventArgs e)
    {
        if (_loading)
            return;
        OpacitySlider.IsEnabled = AcrylicSwitch.IsOn;
        var settings = SettingsStore.Load();
        settings.AcrylicEnabled = AcrylicSwitch.IsOn;
        SettingsStore.Save(settings);
    }

    private void OnOpacityChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        OpacityValue.Text = $"{(int)OpacitySlider.Value}%";
        if (_loading)
            return;
        var settings = SettingsStore.Load();
        settings.BackgroundOpacity = (int)OpacitySlider.Value;
        SettingsStore.Save(settings);
    }

    private void OnApplyTransparency(object sender, RoutedEventArgs e) =>
        App.Main?.ApplyBackdropSettings();

    private void OnResetTransparency(object sender, RoutedEventArgs e)
    {
        AcrylicSwitch.IsOn = true;
        OpacitySlider.Value = 28;
        var settings = SettingsStore.Load();
        settings.AcrylicEnabled = true;
        settings.BackgroundOpacity = 28;
        SettingsStore.Save(settings);
        App.Main?.ApplyBackdropSettings();
    }

    private void OnStartupToggled(object sender, RoutedEventArgs e)
    {
        if (_loading)
            return;
        StartupRegistry.SetEnabled(StartupSwitch.IsOn);
        var settings = SettingsStore.Load();
        settings.StartWithWindows = StartupSwitch.IsOn;
        SettingsStore.Save(settings);
    }

    private void OnUpdatesToggled(object sender, RoutedEventArgs e)
    {
        if (_loading)
            return;
        var settings = SettingsStore.Load();
        settings.CheckUpdatesOnLaunch = UpdatesSwitch.IsOn;
        SettingsStore.Save(settings);
    }

    private async void OnCheckUpdates(object sender, RoutedEventArgs e)
    {
        CheckUpdatesButton.IsEnabled = false;
        UpdateSpinner.IsActive = true;
        UpdateStatus.Text = Loc.Get("Settings_UpdateChecking");
        var check = await UpdateService.CheckAsync();
        UpdateSpinner.IsActive = false;
        CheckUpdatesButton.IsEnabled = true;
        ApplyUpdateCheck(check);
    }

    private void ApplyUpdateCheck(UpdateCheck check)
    {
        UpdateStatus.Text = check.Kind switch
        {
            UpdateCheckKind.NotInstalled => Loc.Get("Settings_UpdateDevBuild"),
            UpdateCheckKind.UpToDate => Loc.Get("Settings_UpdateCurrent"),
            UpdateCheckKind.Failed => Loc.Get("Settings_UpdateFailed"),
            UpdateCheckKind.Available => string.Format(
                Loc.Get("Settings_UpdateReady"),
                check.Offer?.Version
            ),
            _ => string.Empty,
        };

        if (check.Kind == UpdateCheckKind.Available && check.Offer is not null)
            App.Main?.ShowUpdate(check.Offer);
    }

    private void OnQuit(object sender, RoutedEventArgs e) => App.Main?.ExitApplication();
}
