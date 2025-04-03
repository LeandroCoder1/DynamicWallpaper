using System.Runtime.InteropServices;

namespace DynamicWallpaper.Core
{
    public class WallpaperManager
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        private static readonly string WallpaperDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Wallpapers");
    
        public static void Initialize()
        {
            if (!Directory.Exists(WallpaperDirectory))
            {
                Directory.CreateDirectory(WallpaperDirectory);
            }

            System.Diagnostics.Debug.WriteLine("WallpaperManager inicializado.");
        }

        public static IEnumerable<string> GetWallpapers()
        {
            if (!Directory.Exists(WallpaperDirectory))
            {
                Directory.CreateDirectory(WallpaperDirectory);
            }
            return Directory.GetFiles(WallpaperDirectory, "*.jpg").Concat(Directory.GetFiles(WallpaperDirectory, "*.png"));
        }

        public static void SetWallpaper(string wallpaper)
        {
            string filePath = Path.Combine(WallpaperDirectory, wallpaper);

            if (!File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine($"Wallpaper não encontrado: {filePath}");
                return;
            }

            // Define um wallpaper de imagem
            int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            if (result == 0)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao definir o wallpaper.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Wallpaper aplicado: {filePath}");
            }
        }

        public static string[] GetAvailableWallpapers()
        {
            // Retorna a lista de imagens disponíveis na pasta de wallpapers
            if (!Directory.Exists(WallpaperDirectory)) return new string[0];

            return Directory.GetFiles(WallpaperDirectory, "DynamicWallpaper/DynamicWallpaper.Core/Assets/Wallpapers")
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .Select(Path.GetFileName)
                .ToArray();
        }
    }
}
