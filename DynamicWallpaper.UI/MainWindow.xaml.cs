using System.IO;
using System.Windows;
using Microsoft.Win32;
using DynamicWallpaper.Core;
using System.Windows.Controls;
using DynamicWallpaper.Service;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using DynamicWallpaper.UI.src.Screens;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Runtime.InteropServices;
using MahApps.Metro.Controls.Dialogs;

namespace DynamicWallpaper.UI
{
    public partial class MainWindow : Window
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        private readonly WallpaperService _wallpaperService;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        string wallpapersPath = "D:\\leo\\source\\repos\\DynamicWallpaper\\DynamicWallpaper.UI\\Assets\\Wallpapers";
        private List<string> availableWallpapers = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            LoadWallpapers();
        }

        private void LoadWallpapers()
        {
            if (Directory.Exists(wallpapersPath))
            {
                string[] imageFiles = Directory.GetFiles(wallpapersPath, "*.*", SearchOption.TopDirectoryOnly)
                                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                           f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                .ToArray();

                availableWallpapers = imageFiles.ToList();

                WallpaperPanel.Children.Clear();

                double imageSize = Math.Max(200, Math.Min(this.ActualWidth / 5, 200));
                 
                foreach (var imagePath in imageFiles)
                {
                    DropShadowEffect shadowEffect = new DropShadowEffect
                    {
                        Color = Colors.Red,
                        ShadowDepth = 0,
                        BlurRadius = 50,
                        Opacity = 0.5
                    };

                    Border border = new Border
                    {
                        Width = imageSize,
                        Height = 170,
                        Margin = new Thickness(5),
                        BorderBrush = Brushes.Blue,
                        BorderThickness = new Thickness(2),
                        CornerRadius = new CornerRadius(10),
                        ClipToBounds = true,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Effect = shadowEffect,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    TransformGroup transformGroup = new TransformGroup();
                    ScaleTransform scaleTransform = new ScaleTransform();
                    RotateTransform rotateTransform = new RotateTransform();
                    transformGroup.Children.Add(scaleTransform);
                    transformGroup.Children.Add(rotateTransform);
                    border.RenderTransform = transformGroup;

                    Image image = new Image
                    {
                        Width = imageSize,
                        Height = imageSize
                    };

                    BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                    image.Source = bitmap;

                    border.MouseEnter += (sender, e) =>
                    {
                        var scaleAnimation = new DoubleAnimation(1, 1.1, TimeSpan.FromSeconds(0.3));
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

                        var rotateAnimation = new DoubleAnimation(0, 10, TimeSpan.FromSeconds(0.3));
                        rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

                        var shadowAnimation = new DoubleAnimation(3, 10, TimeSpan.FromSeconds(0.3));
                        shadowEffect.BeginAnimation(DropShadowEffect.ShadowDepthProperty, shadowAnimation);
                    };

                    border.MouseLeave += (sender, e) =>
                    {
                        var scaleAnimation = new DoubleAnimation(1.1, 1, TimeSpan.FromSeconds(0.3));
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

                        var rotateAnimation = new DoubleAnimation(10, 0, TimeSpan.FromSeconds(0.3));
                        rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

                        var shadowAnimation = new DoubleAnimation(10, 3, TimeSpan.FromSeconds(0.3));
                        shadowEffect.BeginAnimation(DropShadowEffect.ShadowDepthProperty, shadowAnimation);
                    };

                    border.MouseLeftButtonUp += (sender, e) =>
                    {
                        // Ao clicar, aplicar o wallpaper correspondente
                        SetWallpaper(imagePath);
                    };

                    border.Child = image;

                    WallpaperPanel.Children.Add(border);
                }

                WallpaperPanel.Opacity = 1.0;
            }
            else
            {
                MessageBox.Show("Deu erro ze");
            }
        }

        private void RefreshWallpapers(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOutAnimation.Completed += (s, _) =>
            {
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
            if (!Directory.Exists(wallpapersPath))
            {
                MessageBox.Show("A pasta de wallpapers não foi encontrada!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var imageFiles = Directory.GetFiles(wallpapersPath, "*.*")
                          .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png"))
                          .ToList();

            if (imageFiles.Count == 0)
            {
                MessageBox.Show("Nenhum wallpaper encontrado!", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = wallpapersPath,
                Filter = "Imagens|*.jpg;*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SetWallpaper(openFileDialog.FileName);
            }
        }

/*        private void ApplyWallpaper(object sender, RoutedEventArgs e)
        {
            if (WallpaperList.SelectedItem != null)
            {
                string selectedWallpaper = Path.Combine(wallpapersPath, WallpaperList.SelectedItem.ToString());
                SetWallpaper(selectedWallpaper);
            }
            else
            {
                MessageBox.Show("deu erro ze", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
*/
        private void SetWallpaper(string filePath)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
    }
}