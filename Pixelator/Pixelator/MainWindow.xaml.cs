using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;



namespace Pixelator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PixelatorModel _pixelator = null;
        private int _pixelCount = 1;

        public MainWindow()
        {
            InitializeComponent();

            _pixelator = new PixelatorModel();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);

                BitmapImage bitmapImage = new BitmapImage(fileUri);

                _pixelator.SetOriginImage(bitmapImage);

                OriginalImage.Source = bitmapImage;
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (OriginalImage.Source != null)
            {
                PixelatedImage.Source = _pixelator.Pixelate(_pixelCount, true);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (PixelatedImage.Source != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    _pixelator.Export(saveFileDialog.FileName);
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            OriginalImage.Source = null;

            PixelatedImage.Source = null;

            OneRButton.IsChecked = true;
        }

        private void OneRButton_Checked(object sender, RoutedEventArgs e)
        {
            _pixelCount = 1;
        }

        private void TwoRButton_Checked(object sender, RoutedEventArgs e)
        {
            _pixelCount = 2;
        }

        private void FourRButton_Checked(object sender, RoutedEventArgs e)
        {
            _pixelCount = 4;
        }

        private void EightRButton_Checked(object sender, RoutedEventArgs e)
        {
            _pixelCount = 8;
        }

        private void SixteenRButton_Checked(object sender, RoutedEventArgs e)
        {
            _pixelCount = 16;
        }
    }
}
