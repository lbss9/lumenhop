namespace Lumenhop;

/// <summary>Copies a user-picked image into the app data folder.</summary>
public static class IconStore
{
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg", ".ico", ".webp"];

    public static bool IsManaged(string? path) =>
        PathGuard.IsInsideDirectory(path, SettingsStore.IconsDirectory);

    public static string? Import(string sourcePath, string targetId)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            return null;
        if (!PathGuard.IsSafeFileStem(targetId))
            return null;

        var extension = Path.GetExtension(sourcePath).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return null;

        return SafeTry.Run(() =>
        {
            Directory.CreateDirectory(SettingsStore.IconsDirectory);
            var destination = Path.GetFullPath(
                Path.Combine(SettingsStore.IconsDirectory, targetId + extension)
            );
            if (!PathGuard.IsInsideDirectory(destination, SettingsStore.IconsDirectory))
                return null;

            File.Copy(sourcePath, destination, overwrite: true);
            return destination;
        });
    }

    public static void Delete(string? path)
    {
        if (!IsManaged(path))
            return;

        SafeTry.Run(() =>
        {
            if (File.Exists(path))
                File.Delete(path);
            return true;
        });
    }
}
