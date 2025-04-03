using DynamicWallpaper.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DynamicWallpaper.UI.src.Screens
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private readonly WallpaperService _wallpaperService;
        private readonly string _wallpaperPath;

        public PreviewWindow(string wallPath, WallpaperService wallpaperService)
        {
            InitializeComponent();
            _wallpaperService = wallpaperService;
            _wallpaperPath = wallPath;
            PreviewImage.Source = new BitmapImage(new Uri(_wallpaperPath));
        }

        private void ApplyWallpaper(object sender, RoutedEventArgs e)
        {
            _wallpaperService.SetSpecificWallpaper(_wallpaperPath);
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
