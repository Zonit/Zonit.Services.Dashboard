using MudBlazor;

namespace Zonit.Services.Dashboard;

public class Theme
{
    public static MudTheme MainLayout => new()
    {
        PaletteLight = new PaletteLight
        {
            // Główne kolory dla AI
            Primary = "#7764e5",          // Odcień fioletowy - główny kolor akcji
            Secondary = "#38bdf8",        // Jasny niebieski - informacje, dane
            Tertiary = "#9333ea",         // Głębszy fiolet - akcenty
            Info = "#3299ff",             // Niebieski - informacje
            Success = "#0bba83",          // Zieleń - powodzenie
            Warning = "#ffa800",          // Pomarańcz - ostrzeżenie
            Error = "#f64e62",            // Czerwony - błąd
            Dark = "#2a2b3c",             // Ciemny - do kontrastów
            Black = "#111111",            // Czarny - tekst i kontrasty

            // Tła i powierzchnie
            Background = "#f8f9fc",       // Jasne, delikatne tło
            BackgroundGray = "#f0f1f5",   // Jaśniejszy szary
            Surface = "#ffffff",          // Biała powierzchnia
            AppbarBackground = "#ffffff", // Białe tło appbara
            DrawerBackground = "#ffffff", // Białe tło szuflady

            // Teksty
            TextPrimary = "#1f2937",      // Główny tekst
            TextSecondary = "#4b5563",    // Drugorzędny tekst
            TextDisabled = "rgba(0,0,0,0.38)",  // Wyłączony tekst
            AppbarText = "#424c5f",       // Tekst appbara
            DrawerText = "#4b5563",       // Tekst szuflady
            DrawerIcon = "#4b5563",       // Ikona szuflady

            // Akcje i interfejs
            ActionDefault = "#64748b",             // Domyślny kolor akcji
            ActionDisabled = "rgba(0,0,0,0.26)",   // Wyłączona akcja
            ActionDisabledBackground = "rgba(0,0,0,0.12)", // Tło wyłączonej akcji

            // Linie i obramowania
            LinesDefault = "rgba(0,0,0,0.12)",     // Domyślne linie
            LinesInputs = "rgba(0,0,0,0.3)",       // Linie inputów
            TableLines = "rgba(0,0,0,0.12)",       // Linie tabeli
            TableStriped = "rgba(0,0,0,0.02)",     // Paski tabeli
            Divider = "rgba(0,0,0,0.12)",          // Separator
            DividerLight = "rgba(0,0,0,0.06)",     // Lekki separator
            Skeleton = "rgba(0,0,0,0.11)",         // Szkielet (placeholder)

            // Poziomy półprzezroczystości
            HoverOpacity = 0.06,
            RippleOpacity = 0.10,
            RippleOpacitySecondary = 0.38,
        },
        PaletteDark = new PaletteDark
        {
            // Główne kolory dla AI - jaśniejsze w trybie ciemnym
            Primary = "#8a7deb",          // Jaśniejszy fiolet - główny kolor akcji
            Secondary = "#56c8fa",        // Jaśniejszy niebieski - informacje
            Tertiary = "#a855f7",         // Jaśniejszy fiolet - akcenty
            Info = "#4dabff",             // Jaśniejszy niebieski - informacje
            Success = "#22d69a",          // Jaśniejszy zielony - powodzenie
            Warning = "#ffc233",          // Jaśniejszy pomarańczowy - ostrzeżenie
            Error = "#f87785",            // Jaśniejszy czerwony - błąd
            Dark = "#27272f",             // Ciemny - do kontrastów
            Black = "#27272f",            // Czarny - tekst i kontrasty

            // Tła i powierzchnie - ciemniejsze, z fioletowym odcieniem
            Background = "#161723",        // Tło z odcieniem fioletu
            BackgroundGray = "#1f2033",    // Ciemny szary z odcieniem fioletu
            Surface = "#232438",           // Powierzchnia z lekkim odcieniem fioletu
            AppbarBackground = "#1c1d30",  // Ciemniejsze tło appbara
            DrawerBackground = "#1c1d30",  // Ciemniejsze tło szuflady

            // Teksty - jaśniejsze dla lepszej widoczności
            TextPrimary = "rgba(255,255,255,0.87)",   // Główny tekst
            TextSecondary = "rgba(255,255,255,0.60)", // Drugorzędny tekst
            TextDisabled = "rgba(255,255,255,0.38)",  // Wyłączony tekst
            AppbarText = "rgba(255,255,255,0.87)",    // Tekst appbara
            DrawerText = "rgba(255,255,255,0.70)",    // Tekst szuflady
            DrawerIcon = "rgba(255,255,255,0.70)",    // Ikona szuflady

            // Akcje i interfejs
            ActionDefault = "#b0b3c9",                   // Domyślny kolor akcji
            ActionDisabled = "rgba(255,255,255,0.26)",   // Wyłączona akcja
            ActionDisabledBackground = "rgba(255,255,255,0.12)", // Tło wyłączonej akcji

            // Linie i obramowania
            LinesDefault = "rgba(255,255,255,0.12)",    // Domyślne linie
            LinesInputs = "rgba(255,255,255,0.3)",      // Linie inputów
            TableLines = "rgba(255,255,255,0.12)",      // Linie tabeli
            TableStriped = "rgba(255,255,255,0.02)",    // Paski tabeli
            Divider = "rgba(255,255,255,0.12)",         // Separator
            DividerLight = "rgba(255,255,255,0.06)",    // Lekki separator
            Skeleton = "rgba(255,255,255,0.11)",        // Szkielet (placeholder)

            // Poziomy półprzezroczystości
            HoverOpacity = 0.08,
            RippleOpacity = 0.12,
            RippleOpacitySecondary = 0.38,
        },

        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1rem",
                LineHeight = "1.6",
                FontWeight = "400"
            },
            H1 = new H1Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1.75rem",
                FontWeight = "600",
                LineHeight = "1.5",
                LetterSpacing = "-0.01em",
            },
            H2 = new H2Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = "600",
                LineHeight = "1.5",
                LetterSpacing = "-0.01em",
            },
            H3 = new H3Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1.35rem",
                FontWeight = "600",
                LineHeight = "1.4",
                LetterSpacing = "-0.005em"
            },
            H4 = new H4Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = "500",
                LineHeight = "1.4",
                LetterSpacing = "-0.005em"
            },
            H5 = new H5Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1.15rem",
                FontWeight = "500",
                LineHeight = "1.4"
            },
            H6 = new H6Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1.05rem",
                FontWeight = "500",
                LineHeight = "1.3"
            },
            Subtitle1 = new Subtitle1Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1rem",
                FontWeight = "500",
                LineHeight = "1.6"
            },
            Subtitle2 = new Subtitle2Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "0.9rem",
                FontWeight = "500",
                LineHeight = "1.6"
            },
            Body1 = new Body1Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "1rem",
                FontWeight = "400",
                LineHeight = "1.6"
            },
            Body2 = new Body2Typography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "0.9rem",
                FontWeight = "400",
                LineHeight = "1.6"
            },
            Button = new ButtonTypography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "0.95rem",
                FontWeight = "500",
                LineHeight = "1.5",
                LetterSpacing = "0.02em",
            },
            Caption = new CaptionTypography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "0.8rem",
                FontWeight = "400",
                LineHeight = "1.4"
            },
            Overline = new OverlineTypography()
            {
                FontFamily = new[] { "Inter", "Roboto", "system-ui", "sans-serif" },
                FontSize = "0.75rem",
                FontWeight = "500",
                LineHeight = "1.4",
                LetterSpacing = "0.08em",
                TextTransform = "uppercase"
            }
        },
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "12px",
        },
    };
}
