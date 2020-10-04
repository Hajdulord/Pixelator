using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Pixelator
{
    class PixelatorModel
    {
        private Bitmap _originImage = null;
        private Bitmap _pixelatedImage = null;

        public void SetOriginImage(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                _originImage = bitmap;
            }
        }

        public BitmapImage Pixelate(int pixelSize)
        {

            if (pixelSize == 1)
            {
                _pixelatedImage = new Bitmap(_originImage);
                return ToBitmapImage(_originImage);
            }

            _pixelatedImage = new Bitmap(_originImage.Width, _originImage.Height);

            for (int xx = 0; xx < _originImage.Width; xx += pixelSize)
            {
                for (int yy = 0; yy < _originImage.Height; yy += pixelSize)
                {
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    int a = 0;

                    for (int x = xx; x < xx + pixelSize && x < _originImage.Width; x++)
                    {
                        for (int y = yy; y < yy + pixelSize && y < _originImage.Height; y++)
                        {
                            var pixel = _originImage.GetPixel(x, y);
                            r += pixel.R;
                            g += pixel.G;
                            b += pixel.B;
                            a += pixel.A;
                        }
                    }
                    r /= pixelSize * pixelSize;
                    g /= pixelSize * pixelSize;
                    b /= pixelSize * pixelSize;
                    a /= pixelSize * pixelSize;

                    var color = Color.FromArgb(a, r, g, b);

                    SetColorInSection(_pixelatedImage, xx, yy, pixelSize, color);
                }
            }

            return ToBitmapImage(_pixelatedImage);
        }

        public void SetColorInSection(Bitmap bitmap, int x, int y , int size, Color color)
        {
            for (int i = x; i < x + size && i < bitmap.Width; i++)
            {
                for (int j = y; j < y + size && j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, color);
                }
            }
        }

        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (Bitmap tempBitmap = new Bitmap(bitmap))
            {
                //bitmap.Dispose();

                MemoryStream ms = new MemoryStream();
                tempBitmap?.Save(ms, ImageFormat.Png);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();

                return image;
            }
        }

        public void Export(string fileName)
        {
            _pixelatedImage.Save(fileName, ImageFormat.Png);
        }

    }
}
