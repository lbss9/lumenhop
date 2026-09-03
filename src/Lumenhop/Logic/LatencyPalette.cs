using System.Text.RegularExpressions;

namespace Lumenhop;

/// <summary>
/// User-defined mapping from round-trip latency to a dot colour.
/// Four bands: good (≤ <see cref="GoodMax"/>), fair (≤ <see cref="FairMax"/>),
/// poor (≤ <see cref="PoorMax"/>), and bad (above).
/// </summary>
public sealed partial class LatencyPalette
{
    public const string DefaultGoodColor = "#2EE6C7";
    public const string DefaultFairColor = "#5B9DFF";
    public const string DefaultPoorColor = "#E3B341";
    public const string DefaultBadColor = "#FF5C7A";

    public const int DefaultGoodMax = 5;
    public const int DefaultFairMax = 15;
    public const int DefaultPoorMax = 30;

    public const int MinThreshold = 1;
    public const int MaxThreshold = 2000;

    public int GoodMax { get; set; } = DefaultGoodMax;
    public int FairMax { get; set; } = DefaultFairMax;
    public int PoorMax { get; set; } = DefaultPoorMax;

    public string GoodColor { get; set; } = DefaultGoodColor;
    public string FairColor { get; set; } = DefaultFairColor;
    public string PoorColor { get; set; } = DefaultPoorColor;
    public string BadColor { get; set; } = DefaultBadColor;

    public static LatencyPalette Default => new();

    /// <summary>Colour for a measured latency, honouring the current band edges.</summary>
    public string ColorFor(long roundtripMs)
    {
        if (roundtripMs <= GoodMax)
            return GoodColor;
        if (roundtripMs <= FairMax)
            return FairColor;
        if (roundtripMs <= PoorMax)
            return PoorColor;
        return BadColor;
    }

    /// <summary>Returns a copy with ordered, in-range thresholds and valid colours.</summary>
    public LatencyPalette Normalized()
    {
        var good = Math.Clamp(GoodMax, MinThreshold, MaxThreshold - 2);
        var fair = Math.Clamp(FairMax, good + 1, MaxThreshold - 1);
        var poor = Math.Clamp(PoorMax, fair + 1, MaxThreshold);

        return new LatencyPalette
        {
            GoodMax = good,
            FairMax = fair,
            PoorMax = poor,
            GoodColor = Sanitize(GoodColor, DefaultGoodColor),
            FairColor = Sanitize(FairColor, DefaultFairColor),
            PoorColor = Sanitize(PoorColor, DefaultPoorColor),
            BadColor = Sanitize(BadColor, DefaultBadColor),
        };
    }

    public LatencyPalette Clone() =>
        new()
        {
            GoodMax = GoodMax,
            FairMax = FairMax,
            PoorMax = PoorMax,
            GoodColor = GoodColor,
            FairColor = FairColor,
            PoorColor = PoorColor,
            BadColor = BadColor,
        };

    public static bool IsValidColor(string? hex) =>
        !string.IsNullOrWhiteSpace(hex) && HexPattern().IsMatch(hex.Trim());

    private static string Sanitize(string? hex, string fallback) =>
        IsValidColor(hex) ? hex!.Trim().ToUpperInvariant() : fallback;

    [GeneratedRegex(@"^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexPattern();
}
