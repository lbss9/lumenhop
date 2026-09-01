namespace Lumenhop.Tests;

public sealed class LatencyFormatTests
{
    [Fact]
    public void Format_probing_returns_ellipsis()
    {
        Assert.Equal("…", LatencyFormat.Format(12, PingState.Probing));
    }

    [Fact]
    public void Format_idle_returns_dash()
    {
        Assert.Equal("—", LatencyFormat.Format(null, PingState.Idle));
    }

    [Fact]
    public void Format_off_returns_dash()
    {
        Assert.Equal("—", LatencyFormat.Format(12, PingState.Off));
    }

    [Fact]
    public void Format_down_returns_off()
    {
        Assert.Equal("off", LatencyFormat.Format(null, PingState.Down));
    }

    [Fact]
    public void Format_online_uses_milliseconds()
    {
        Assert.Equal("15ms", LatencyFormat.Format(15, PingState.Online));
    }

    [Fact]
    public void Format_large_rtt_stays_in_milliseconds()
    {
        Assert.Equal("1500ms", LatencyFormat.Format(1500, PingState.Slow));
    }
}
