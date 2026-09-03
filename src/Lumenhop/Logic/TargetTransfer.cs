using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Lumenhop;

/// <summary>Reads and writes the shareable <c>.lumenhop</c> targets file.</summary>
public static class TargetTransfer
{
    public const string Signature = "lumenhop.targets";
    public const string FileExtension = ".lumenhop";
    public const int FormatVersion = 1;
    public const int MaxTargets = 200;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    public static string Export(IEnumerable<PingTarget> targets, string appVersion)
    {
        var items = targets
            .Where(TargetValidator.IsValid)
            .Take(MaxTargets)
            .Select(t => new TransferItem
            {
                Title = t.Title,
                Host = t.Host,
                IconGlyph = TargetIcons.IsKnown(t.IconGlyph) ? t.IconGlyph : TargetIcons.DefaultGlyph,
                PollingSeconds = PollingOptions.Clamp(t.PollingSeconds),
            })
            .ToList();

        var envelope = new TransferEnvelope
        {
            Format = Signature,
            FormatVersion = FormatVersion,
            App = appVersion,
            ExportedAt = DateTimeOffset.UtcNow.ToString("O"),
            Count = items.Count,
            Checksum = Checksum(items),
            Targets = items,
        };

        return JsonSerializer.Serialize(envelope, JsonOptions);
    }

    public static TargetImport Import(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return TargetImport.Fail(ImportError.Empty);

        TransferEnvelope? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<TransferEnvelope>(content, JsonOptions);
        }
        catch
        {
            return TargetImport.Fail(ImportError.Unreadable);
        }

        if (envelope is null || !string.Equals(envelope.Format, Signature, StringComparison.Ordinal))
            return TargetImport.Fail(ImportError.Unrecognized);
        if (envelope.FormatVersion != FormatVersion)
            return TargetImport.Fail(ImportError.Version);

        var items = envelope.Targets ?? [];
        if (!string.Equals(envelope.Checksum, Checksum(items), StringComparison.OrdinalIgnoreCase))
            return TargetImport.Fail(ImportError.Tampered);

        var result = new List<PingTarget>();
        foreach (var item in items.Take(MaxTargets))
        {
            var title = (item.Title ?? string.Empty).Trim();
            if (title.Length > TargetValidator.TitleMaxLength)
                title = title[..TargetValidator.TitleMaxLength];

            var target = new PingTarget
            {
                Title = title,
                Host = TargetValidator.NormalizeHost(item.Host),
                IconGlyph = TargetIcons.IsKnown(item.IconGlyph)
                    ? item.IconGlyph!
                    : TargetIcons.DefaultGlyph,
                IconPath = null,
                PollingSeconds = PollingOptions.Clamp(item.PollingSeconds),
                IsEnabled = true,
            };

            if (TargetValidator.IsValid(target))
                result.Add(target);
        }

        return TargetImport.Success(result);
    }

    private static string Checksum(List<TransferItem> items)
    {
        var canonical = string.Join(
            '␞',
            items.Select(i =>
                string.Join(
                    '␟',
                    i.Title ?? string.Empty,
                    i.Host ?? string.Empty,
                    i.IconGlyph ?? string.Empty,
                    i.PollingSeconds.ToString()
                )
            )
        );
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return Convert.ToHexString(hash);
    }

    private sealed class TransferEnvelope
    {
        public string? Format { get; set; }
        public int FormatVersion { get; set; }
        public string? App { get; set; }
        public string? ExportedAt { get; set; }
        public int Count { get; set; }
        public string? Checksum { get; set; }
        public List<TransferItem>? Targets { get; set; }
    }

    private sealed class TransferItem
    {
        public string? Title { get; set; }
        public string? Host { get; set; }
        public string? IconGlyph { get; set; }
        public int PollingSeconds { get; set; }
    }
}

public enum ImportError
{
    None,
    Empty,
    Unreadable,
    Unrecognized,
    Version,
    Tampered,
}

/// <summary>Outcome of reading a <c>.lumenhop</c> file.</summary>
public sealed class TargetImport
{
    private TargetImport(bool ok, ImportError error, IReadOnlyList<PingTarget> targets)
    {
        Ok = ok;
        Error = error;
        Targets = targets;
    }

    public bool Ok { get; }
    public ImportError Error { get; }
    public IReadOnlyList<PingTarget> Targets { get; }

    public static TargetImport Fail(ImportError error) => new(false, error, []);

    public static TargetImport Success(IReadOnlyList<PingTarget> targets) =>
        new(true, ImportError.None, targets);
}
