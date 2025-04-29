using MudBlazor;

namespace Zonit.Services.Dashboard;

public class Theme
{
    public static MudTheme MainLayout => new()
    {
        PaletteLight = new()
        {
            AppbarBackground = "#ffffff",
            AppbarText = "#65676b",
            Background = "#f0f2f5",
            DrawerBackground = "#fdfdfd",
            Surface = "#ffffff",
            TextPrimary = "#050505",

        },
        PaletteDark = new()
        {
            AppbarBackground = "#242526",
            AppbarText = "#b0b3b8",
            Background = "#18191a",
            DrawerBackground = "#212223",
            Surface = "#242526",
            TextPrimary = "#E4E6EB"
        },

        Typography = new Typography()
        {
            H1 = new H1Typography()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = "500",
                LineHeight = "1.6",
                LetterSpacing = ".0075em"
            }
        },
    };
}
