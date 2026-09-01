namespace Lumenhop;

/// <summary>Formats round-trip time for the compact card metric.</summary>
public static class LatencyFormat
{
    public static string Format(long? roundtripMs, PingState state)
    {
        if (state == PingState.Probing)
            return "…";

        if (state is PingState.Idle or PingState.Off)
            return "—";

        if (state == PingState.Down || roundtripMs is null)
            return "off";

        return $"{roundtripMs}ms";
    }
}
