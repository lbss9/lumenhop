using System.Text.Json;

namespace Lumenhop;

/// <summary>Loads and saves <see cref="AppSettings"/> as JSON in LocalAppData.</summary>
public static class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    public static string DirectoryPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Lumenhop"
        );

    public static string FilePath => Path.Combine(DirectoryPath, "settings.json");

    public static string IconsDirectory => Path.Combine(DirectoryPath, "icons");

    public static AppSettings Load()
    {
        var loaded = SafeTry.Run(() =>
        {
            if (!File.Exists(FilePath))
                return SeedDefaults();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? SeedDefaults();
        });

        return Sanitize(loaded ?? SeedDefaults());
    }

    public static void Save(AppSettings settings)
    {
        Directory.CreateDirectory(DirectoryPath);
        var json = JsonSerializer.Serialize(Sanitize(settings), JsonOptions);
        File.WriteAllText(FilePath, json);
    }

    public static bool Exists() => File.Exists(FilePath);

    private static AppSettings Sanitize(AppSettings settings)
    {
        settings.Latency = (settings.Latency ?? new LatencyPalette()).Normalized();

        foreach (var target in settings.Targets)
        {
            target.Id = PathGuard.SanitizeId(target.Id);
            target.Title = (target.Title ?? string.Empty).Trim();
            if (target.Title.Length > TargetValidator.TitleMaxLength)
                target.Title = target.Title[..TargetValidator.TitleMaxLength];
            target.Host = TargetValidator.NormalizeHost(target.Host);
            target.PollingSeconds = PollingOptions.Clamp(target.PollingSeconds);
            if (!PathGuard.IsInsideDirectory(target.IconPath, IconsDirectory))
                target.IconPath = null;
        }

        settings.Targets.RemoveAll(target => !TargetValidator.IsValid(target));
        return settings;
    }

    private static AppSettings SeedDefaults() =>
        new()
        {
            Targets =
            [
                new PingTarget
                {
                    Title = "Cloudflare",
                    Host = "1.1.1.1",
                    IconGlyph = "\uE753",
                    PollingSeconds = 5,
                },
                new PingTarget
                {
                    Title = "Google DNS",
                    Host = "8.8.8.8",
                    IconGlyph = "\uE774",
                    PollingSeconds = 5,
                },
            ],
        };
}
