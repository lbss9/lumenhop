using System.Globalization;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources.Core;

namespace Lumenhop;

public partial class App : Application
{
    public static MainWindow? Main { get; private set; }

    public App()
    {
        UnhandledException += (_, e) =>
        {
            try
            {
                var path = Path.Combine(SettingsStore.DirectoryPath, "crash.log");
                Directory.CreateDirectory(SettingsStore.DirectoryPath);
                File.WriteAllText(
                    path,
                    $"{DateTime.UtcNow:O}\n{e.Exception.GetType().FullName}: {e.Message}"
                );
            }
            catch { }
        };
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var settings = SettingsStore.Load();
        if (!SettingsStore.Exists())
            SettingsStore.Save(settings);

        ApplyCulture(settings.Language);
        Main = new MainWindow();
        Main.ApplyTheme(settings.Theme);
        Main.Activate();

        if (settings.CheckUpdatesOnLaunch)
            _ = CheckUpdatesOnLaunchAsync();
    }

    private static async Task CheckUpdatesOnLaunchAsync()
    {
        var check = await UpdateService.CheckAsync();
        if (check.Kind != UpdateCheckKind.Available || check.Offer is null)
            return;
        Main?.DispatcherQueue.TryEnqueue(() => Main.ShowUpdate(check.Offer));
    }

    public static void ApplyCulture(string language)
    {
        try
        {
            var resolved = AppLanguage.Resolve(language);
            var culture = new CultureInfo(resolved);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            ResourceContext.SetGlobalQualifierValue("Language", resolved);
        }
        catch { }
    }
}
