namespace Lumenhop;

/// <summary>Allowed polling intervals shown in the editor and settings.</summary>
public static class PollingOptions
{
    public static readonly int[] Seconds = [1, 2, 5, 10, 15, 30, 60];

    public static int Clamp(int seconds)
    {
        if (Seconds.Contains(seconds))
            return seconds;

        return Seconds.OrderBy(option => Math.Abs(option - seconds)).First();
    }
}
