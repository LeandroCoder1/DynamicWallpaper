using DynamicWallpaper.Core;
using System.Windows;


namespace DynamicWallpaper.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Inicializa os serviços essenciais
            WallpaperManager.Initialize();

            // Inicia a interface gráfica
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

}
