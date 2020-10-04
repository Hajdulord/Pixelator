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

            return null;
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
