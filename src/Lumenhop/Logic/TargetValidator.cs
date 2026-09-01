using System.Net;
using System.Text.RegularExpressions;

namespace Lumenhop;

/// <summary>Validates fields of a ping destination before it is saved.</summary>
public static class TargetValidator
{
    public const int TitleMaxLength = 40;
    public const int HostMaxLength = 253;
    public const int PollingMinSeconds = 1;
    public const int PollingMaxSeconds = 300;

    private static readonly Regex HostPattern = new(
        @"^[a-zA-Z0-9](?:[a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    public static bool IsTitleValid(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return false;
        return title.Trim().Length <= TitleMaxLength;
    }

    public static string NormalizeHost(string? host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return string.Empty;

        var value = host.Trim();
        if (value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            value = value[8..];
        else if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            value = value[7..];

        var cut = value.IndexOfAny(['/', '?', '#']);
        if (cut >= 0)
            value = value[..cut];

        var colon = value.LastIndexOf(':');
        if (colon > 0 && value.IndexOf(':') == colon && int.TryParse(value[(colon + 1)..], out _))
            value = value[..colon];

        return value.Trim().TrimEnd('.');
    }

    public static string TitleFromHost(string? host)
    {
        var value = NormalizeHost(host);
        if (value.Length <= TitleMaxLength)
            return value;
        return value[..TitleMaxLength];
    }

    public static bool IsHostValid(string? host)
    {
        var value = NormalizeHost(host);
        if (value.Length is 0 or > HostMaxLength)
            return false;

        if (IPAddress.TryParse(value, out _))
            return true;

        return HostPattern.IsMatch(value);
    }

    public static bool IsPollingValid(int seconds) =>
        seconds is >= PollingMinSeconds and <= PollingMaxSeconds;

    public static bool IsValid(PingTarget target) =>
        IsTitleValid(target.Title)
        && IsHostValid(target.Host)
        && IsPollingValid(target.PollingSeconds);
}
