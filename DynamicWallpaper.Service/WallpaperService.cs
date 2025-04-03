using DynamicWallpaper.Core;
using System.Diagnostics;
using System.Timers;

namespace DynamicWallpaper.Service
{
    public class WallpaperService
    {
        private readonly System.Timers.Timer _wallpaperUpdateTimer;
        private readonly WallpaperManager _wallpaperManager;
        private List<string> _availableWallpapers;
        private int _currentWallpaperIndex;

        public WallpaperService()
        {
            _wallpaperManager = new WallpaperManager();
            _wallpaperUpdateTimer = new System.Timers.Timer(300000); // Atualiza a cada 5 minutos
            _wallpaperUpdateTimer.Elapsed += UpdateWallpaper;

            LoadAvailableWallpapers();
        }

        public void StartService()
        {
            Logger.Log("Iniciando serviço de wallpapers...");
            _wallpaperUpdateTimer.Start();
            SetNextWallpaper();
        }

        public void StopService()
        {
            Logger.Log("Parando serviço de wallpapers...");
            _wallpaperUpdateTimer.Stop();
        }

        private void UpdateWallpaper(object sender, ElapsedEventArgs e)
        {
            SetNextWallpaper();
        }

        public void SetNextWallpaper()
        {
            if (_availableWallpapers.Count == 0)
            {
                Logger.Log("Nenhum wallpaper disponível!");
                return;
            }

            _currentWallpaperIndex = (_currentWallpaperIndex + 1) % _availableWallpapers.Count;
            string wallpaper = _availableWallpapers[_currentWallpaperIndex];
            Logger.Log($"Aplicando wallpaper: {wallpaper}");
            WallpaperManager.SetWallpaper(wallpaper); // Fixed the error by qualifying with type name
        }

        public void SetSpecificWallpaper(string wallpaper)
        {
            if (File.Exists(wallpaper))
            {
                Logger.Log($"Aplicando wallpaper específico: {wallpaper}");
                WallpaperManager.SetWallpaper(wallpaper); // Fixed the error by qualifying with type name
            }
            else
            {
                Logger.Log("O arquivo do wallpaper não existe!");
            }
        }

        private void LoadAvailableWallpapers()
        {
            string wallpaperDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Wallpapers");
            if (!Directory.Exists(wallpaperDirectory))
            {
                Logger.Log("Pasta de wallpapers não encontrada, criando diretório...");
                Directory.CreateDirectory(wallpaperDirectory);
            }

            _availableWallpapers = new List<string>(Directory.GetFiles(wallpaperDirectory, "*.jpg"));
            _availableWallpapers.AddRange(Directory.GetFiles(wallpaperDirectory, "*.png"));
            _availableWallpapers.AddRange(Directory.GetFiles(wallpaperDirectory, "*.gif"));

            Logger.Log($"{_availableWallpapers.Count} wallpapers carregados.");
        }

        public void SetDynamicWallpaper(string wallpaperPath)
        {
            if (string.IsNullOrEmpty(wallpaperPath)) return;

            // Inicia o DynamicWallpaper.Player para exibir o wallpaper dinâmico
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "DynamicWallpaper.Player.exe",
                Arguments = $"\"{wallpaperPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(startInfo);
        }
    }
}
