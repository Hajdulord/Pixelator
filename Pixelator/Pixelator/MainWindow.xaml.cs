using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pixelator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PixelatorModel pixelator = null;

        public MainWindow()
        {
            InitializeComponent();

            pixelator = new PixelatorModel();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);

                BitmapImage bitmapImage = new BitmapImage(fileUri);

                pixelator.SetOriginImage(bitmapImage);

                OriginalImage.Source = bitmapImage;
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            PixelatedImage.Source = pixelator.Pixelate(1);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                pixelator.Export(dialog.FileName);
            }
        }
    }
}
