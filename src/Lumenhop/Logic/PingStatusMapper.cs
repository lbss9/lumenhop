namespace Lumenhop;

/// <summary>Maps a probe result to the card state used by the UI.</summary>
public static class PingStatusMapper
{
    public const long SlowThresholdMs = 200;

    public static PingState FromProbe(PingProbeResult result)
    {
        if (!result.Success)
            return PingState.Down;

        if (result.RoundtripMs is null)
            return PingState.Down;

        if (result.RoundtripMs >= SlowThresholdMs)
            return PingState.Slow;

        return PingState.Online;
    }
}
