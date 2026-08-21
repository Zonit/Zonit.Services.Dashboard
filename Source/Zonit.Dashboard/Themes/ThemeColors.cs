using System.Globalization;

namespace Zonit.Dashboard.Themes;

/// <summary>
/// Colour maths the built-in themes use to keep a tenant's brand legible in both modes.
/// </summary>
/// <remarks>
/// <para><b>Why this exists.</b> Every built-in theme fed the tenant's brand hex straight into
/// <c>PaletteLight.Primary</c> <em>and</em> <c>PaletteDark.Primary</c>, on the assumption stated in
/// the old comment that "MudBlazor adjusts contrast against the dark surface, so the same hex
/// usually reads fine in both modes". It does not adjust anything. Measured in the browser with the
/// shipped defaults, the tenant's <c>#2563EB</c> scored <b>2.94:1</b> against the dark surface —
/// below even the 3:1 floor for large text, on the colour that paints every link, every primary
/// button label and the active row in the sidebar. That is the whole of "the colours are hopeless,
/// no contrast, too dark".</para>
///
/// <para><b>What it does instead.</b> A brand colour is nudged along its own lightness axis — hue
/// and saturation untouched, so it still reads as the brand — until it clears a target ratio
/// against the surface it will actually be painted on. Lighter on a dark surface, darker on a light
/// one. A colour that already clears the bar is returned unchanged, which is the common case for a
/// well-chosen brand in light mode: the correction only fires where it is needed.</para>
///
/// <para><b>Ratios are WCAG 2.1 relative luminance.</b> The same formula the audit used, so the
/// numbers in the theme files can be re-checked with any contrast tool.</para>
/// </remarks>
internal static class ThemeColors
{
    /// <summary>WCAG AA for body text.</summary>
    public const double AaText = 4.5;

    /// <summary>WCAG AA for large text and for UI component boundaries.</summary>
    public const double AaLarge = 3.0;

    /// <summary>
    /// Returns <paramref name="color"/> lightened or darkened just enough to reach
    /// <paramref name="minRatio"/> against <paramref name="background"/>, preserving hue and
    /// saturation. Returns the input unchanged when it already passes, or when either value cannot
    /// be parsed — a theme must never fail to build because of a malformed tenant setting.
    /// </summary>
    public static string EnsureContrast(string? color, string background, double minRatio)
    {
        if (!TryParse(color, out var fg) || !TryParse(background, out var bg))
            return color ?? "#000000";

        if (Ratio(fg, bg) >= minRatio)
            return ToHex(fg);

        // Move away from the background: lighten on a dark ground, darken on a light one. The
        // direction is decided once, from the background, rather than per step — otherwise a
        // mid-tone colour can oscillate and never converge.
        var lightenTowardsWhite = Luminance(bg) < 0.5;

        var (h, s, l) = ToHsl(fg);

        // 1% steps. 100 iterations is a hard bound on a loop whose input is a tenant setting;
        // the worst realistic case (a near-background colour that has to cross the whole range)
        // converges in well under half of them.
        for (var i = 1; i <= 100; i++)
        {
            var candidateL = lightenTowardsWhite
                ? Math.Min(1.0, l + i * 0.01)
                : Math.Max(0.0, l - i * 0.01);

            var candidate = FromHsl(h, s, candidateL);
            if (Ratio(candidate, bg) >= minRatio)
                return ToHex(candidate);

            // Ran into pure white / pure black without reaching the target: nothing on this hue
            // can do better, so return the extreme rather than the original.
            if (candidateL is <= 0.0 or >= 1.0)
                return ToHex(candidate);
        }

        return ToHex(fg);
    }

    /// <summary>
    /// Picks black or white — whichever reads better on <paramref name="background"/>. Used for the
    /// <c>*-text</c> palette slots, which MudBlazor paints on top of a filled brand colour.
    /// </summary>
    public static string OnColor(string background)
        => TryParse(background, out var bg) && Luminance(bg) > 0.45
            ? "#000000"
            : "#ffffff";

    /// <summary>WCAG 2.1 contrast ratio between two opaque colours. Always ≥ 1.</summary>
    public static double Ratio(Rgb a, Rgb b)
    {
        var la = Luminance(a);
        var lb = Luminance(b);
        var hi = Math.Max(la, lb);
        var lo = Math.Min(la, lb);
        return (hi + 0.05) / (lo + 0.05);
    }

    /// <summary>Convenience overload for two hex strings; returns 1 when either fails to parse.</summary>
    public static double Ratio(string? a, string? b)
        => TryParse(a, out var ca) && TryParse(b, out var cb) ? Ratio(ca, cb) : 1.0;

    /// <summary>Mixes <paramref name="amount"/> (0..1) of <paramref name="b"/> into <paramref name="a"/>.</summary>
    public static string Mix(string a, string b, double amount)
    {
        if (!TryParse(a, out var ca) || !TryParse(b, out var cb))
            return a;

        amount = Math.Clamp(amount, 0, 1);
        return ToHex(new Rgb(
            (byte)Math.Round(ca.R + (cb.R - ca.R) * amount),
            (byte)Math.Round(ca.G + (cb.G - ca.G) * amount),
            (byte)Math.Round(ca.B + (cb.B - ca.B) * amount)));
    }

    // ─── primitives ────────────────────────────────────────────────────────────

    internal readonly record struct Rgb(byte R, byte G, byte B);

    public static bool TryParse(string? value, out Rgb rgb)
    {
        rgb = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var s = value.Trim();
        if (s[0] == '#') s = s[1..];

        // #abc shorthand.
        if (s.Length == 3)
            s = new string([s[0], s[0], s[1], s[1], s[2], s[2]]);

        // #rrggbbaa — alpha is dropped, not blended: these slots are opaque brand colours and a
        // translucent one would make every ratio below a lie.
        if (s.Length == 8) s = s[..6];

        if (s.Length != 6) return false;

        if (!byte.TryParse(s.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) ||
            !byte.TryParse(s.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) ||
            !byte.TryParse(s.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
            return false;

        rgb = new Rgb(r, g, b);
        return true;
    }

    public static string ToHex(Rgb c)
        => string.Create(CultureInfo.InvariantCulture, $"#{c.R:x2}{c.G:x2}{c.B:x2}");

    private static double Luminance(Rgb c)
        => 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);

    private static double Channel(byte raw)
    {
        var v = raw / 255.0;
        return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
    }

    private static (double H, double S, double L) ToHsl(Rgb c)
    {
        double r = c.R / 255.0, g = c.G / 255.0, b = c.B / 255.0;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var l = (max + min) / 2.0;

        if (Math.Abs(max - min) < 1e-9)
            return (0, 0, l);

        var d = max - min;
        var s = l > 0.5 ? d / (2.0 - max - min) : d / (max + min);

        double h;
        if (Math.Abs(max - r) < 1e-9) h = (g - b) / d + (g < b ? 6 : 0);
        else if (Math.Abs(max - g) < 1e-9) h = (b - r) / d + 2;
        else h = (r - g) / d + 4;

        return (h / 6.0, s, l);
    }

    private static Rgb FromHsl(double h, double s, double l)
    {
        if (s <= 0)
        {
            var v = (byte)Math.Round(l * 255);
            return new Rgb(v, v, v);
        }

        var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        var p = 2 * l - q;

        return new Rgb(
            (byte)Math.Round(Hue(p, q, h + 1.0 / 3.0) * 255),
            (byte)Math.Round(Hue(p, q, h) * 255),
            (byte)Math.Round(Hue(p, q, h - 1.0 / 3.0) * 255));

        static double Hue(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
            return p;
        }
    }
}
