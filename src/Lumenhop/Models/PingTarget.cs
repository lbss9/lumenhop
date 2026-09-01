namespace Lumenhop;

/// <summary>Persisted ping destination configured by the user.</summary>
public sealed class PingTarget
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string Title { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string IconGlyph { get; set; } = TargetIcons.DefaultGlyph;

    public string? IconPath { get; set; }

    public int PollingSeconds { get; set; } = 5;

    public bool IsEnabled { get; set; } = true;

    public PingTarget Clone() =>
        new()
        {
            Id = Id,
            Title = Title,
            Host = Host,
            IconGlyph = IconGlyph,
            IconPath = IconPath,
            PollingSeconds = PollingSeconds,
            IsEnabled = IsEnabled,
        };
}
