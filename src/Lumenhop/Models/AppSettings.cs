namespace Lumenhop;

/// <summary>User preferences and the list of monitored destinations.</summary>
public sealed class AppSettings
{
    public List<PingTarget> Targets { get; set; } = [];

    public string Language { get; set; } = AppLanguage.System;

    public string Theme { get; set; } = "System";

    public bool StartWithWindows { get; set; }

    public bool AcrylicEnabled { get; set; } = true;

    public int BackgroundOpacity { get; set; } = 28;

    public int DefaultPollingSeconds { get; set; } = 5;

    public string WindowAnchor { get; set; } = nameof(Lumenhop.WindowAnchor.BottomRight);

    public bool CheckUpdatesOnLaunch { get; set; } = true;

    public LatencyPalette Latency { get; set; } = new();
}
