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
    public partial class SettingsWindow : Window
    {
        public bool IsDarkMode { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();
           // LoadSettings();
            DataContext = this;
        }

        private void LoadSettings()
        {
        }
    }
}
