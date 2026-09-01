using System.Net.NetworkInformation;

namespace Lumenhop;

/// <summary>Sends a single ICMP echo request to a host.</summary>
public static class PingClient
{
    public const int DefaultTimeoutMs = 3000;

    public static async Task<PingProbeResult> ProbeAsync(
        string host,
        int timeoutMs = DefaultTimeoutMs,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(host))
            return new PingProbeResult(false, null, "empty");

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host.Trim(), timeoutMs);
            if (reply.Status == IPStatus.Success)
                return new PingProbeResult(true, reply.RoundtripTime, null);

            return new PingProbeResult(false, null, reply.Status.ToString());
        }
        catch (Exception ex)
        {
            return new PingProbeResult(false, null, ex.Message);
        }
    }
}
