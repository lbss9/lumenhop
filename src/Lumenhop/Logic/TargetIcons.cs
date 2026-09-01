namespace Lumenhop;

/// <summary>Curated Fluent glyphs the user can assign to a destination.</summary>
public static class TargetIcons
{
    public const string DefaultGlyph = "\uE774";

    public static readonly IReadOnlyList<(string Glyph, string Label)> Catalog =
    [
        ("\uE774", "Globo"),
        ("\uE753", "Nuvem"),
        ("\uE704", "Wi-Fi"),
        ("\uE968", "Servidor"),
        ("\uE950", "Rede"),
        ("\uE80F", "Casa"),
        ("\uE7FC", "Jogo"),
        ("\uE8F1", "Mídia"),
        ("\uE8D7", "Celular"),
        ("\uE770", "Mapa"),
        ("\uE8C7", "Pasta"),
        ("\uE90E", "Disco"),
    ];

    public static bool IsKnown(string? glyph) =>
        !string.IsNullOrEmpty(glyph) && Catalog.Any(item => item.Glyph == glyph);
}
