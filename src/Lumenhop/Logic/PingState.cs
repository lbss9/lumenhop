namespace Lumenhop;

/// <summary>Visual and logical state of a single ping destination.</summary>
public enum PingState
{
    Idle,
    Off,
    Probing,
    Online,
    Slow,
    Down,
}
