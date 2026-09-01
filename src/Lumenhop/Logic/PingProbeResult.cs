namespace Lumenhop;

/// <summary>Outcome of one ICMP probe.</summary>
public readonly record struct PingProbeResult(bool Success, long? RoundtripMs, string? Error);
