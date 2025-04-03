using System.IO;
using System.Windows;
using Microsoft.Win32;
using DynamicWallpaper.Core;
using System.Windows.Controls;
using DynamicWallpaper.Service;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using DynamicWallpaper.UI.src.Screens;

namespace DynamicWallpaper.UI
{
    public partial class MainWindow : Window
    {
        private readonly WallpaperService _wallpaperService;

        public MainWindow()
        {
            InitializeComponent();
            LoadWallpapers();
        }

        private void LoadWallpapers()
        {
            string[] wallpapers = WallpaperManager.GetAvailableWallpapers();

            foreach (string wallpaper in wallpapers)
            {
                var image = new Image()
                {
                    Source = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Wallpapers", wallpaper))),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(5),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                image.MouseLeftButtonUp += (s, e) => WallpaperManager.SetWallpaper(System.IO.Path.GetFileName(wallpaper));
                WallpaperPanel.Children.Add(image);
            }
        }

        private void RefreshWallpapers(object sender, RoutedEventArgs e)
        {
            // Criando animação de fade-out antes da atualização
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOutAnimation.Completed += (s, _) =>
            {
                // Limpa os wallpapers e recarrega a lista
                WallpaperPanel.Children.Clear();
                LoadWallpapers();
            };

            WallpaperPanel.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void OnWallpapersLoaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            WallpaperPanel.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void SelectWallpaper(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Select Wallpaper"
            };

            if(openFileDialog.ShowDialog() == true)
            {
                AnimateWallpaperTransiction(() => _wallpaperService.SetSpecificWallpaper(openFileDialog.FileName));

                PreviewWindow previewWindow = new PreviewWindow(openFileDialog.FileName, _wallpaperService);
                previewWindow.ShowDialog();
            }
        }

        private void AnimateWallpaperTransiction(Action setWallpaperAction)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            DoubleAnimation fadeIn = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));

            fadeOut.Completed += (s, e) =>
            {
                setWallpaperAction.Invoke();
                WallpaperPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }
    }
}