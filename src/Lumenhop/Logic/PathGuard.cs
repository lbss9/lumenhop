namespace Lumenhop;

/// <summary>Keeps resolved file paths inside an allowed directory.</summary>
public static class PathGuard
{
    public static bool IsSafeFileStem(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        if (name.Length is < 8 or > 64)
            return false;
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return false;
        return name.All(char.IsAsciiHexDigit);
    }

    public static string SanitizeId(string? id) =>
        IsSafeFileStem(id) ? id! : Guid.NewGuid().ToString("N");

    public static bool IsInsideDirectory(string? path, string? directory)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(directory))
            return false;

        var fullPath = SafeTry.Run(() => Path.GetFullPath(path));
        var root = SafeTry.Run(() => Path.GetFullPath(directory));
        if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(root))
            return false;

        var endsWithSeparator =
            root.EndsWith(Path.DirectorySeparatorChar)
            || root.EndsWith(Path.AltDirectorySeparatorChar);
        if (!endsWithSeparator)
            root += Path.DirectorySeparatorChar;

        return fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase);
    }
}
