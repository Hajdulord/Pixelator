using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Pixelator
{
    class PixelatorModel
    {
        private Bitmap _originImage = null;
        private Bitmap _pixelatedImage = null;
        private MemoryStream _convertMemoryStream = new MemoryStream();

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
                    var r = 0;
                    var g = 0;
                    var b = 0;
                    var a = 0;

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

        public BitmapImage Pixelate(int pixelSize, bool isParallel)
        {
            if (!isParallel || pixelSize == 1) return Pixelate(pixelSize);

            _pixelatedImage = new Bitmap(_originImage);

            var rect = new Rectangle(0, 0, _pixelatedImage.Width, _pixelatedImage.Height);
            var data = _pixelatedImage.LockBits(rect, ImageLockMode.ReadWrite, _pixelatedImage.PixelFormat);
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8;

            var buffer = new byte[data.Width * data.Height * depth];

            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            Parallel.Invoke(
                () => {
                    //upper-left
                    Process(buffer, 0, 0, data.Width / 2, data.Height / 2, data.Width, depth, pixelSize);
                },
                () => {
                    //upper-right
                    Process(buffer, data.Width / 2, 0, data.Width, data.Height / 2, data.Width, depth, pixelSize);
                },
                () => {
                    //lower-right
                    Process(buffer, data.Width / 2, data.Height / 2, data.Width, data.Height, data.Width, depth, pixelSize);
                },
                () => {
                    //lower-left
                    Process(buffer, 0, data.Height / 2, data.Width / 2, data.Height , data.Width , depth, pixelSize);
                }
            );
            
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);

            _pixelatedImage.UnlockBits(data);

            return ToBitmapImage(_pixelatedImage);
        }

        private void Process(byte[] buffer, int x, int y, int endx, int endy, int width, int depth, int pixelSize)
        {
            for (int i = x; i < endx; i += pixelSize)
            {
                for (int j = y; j < endy; j += pixelSize)
                {
                    var r = 0;
                    var g = 0;
                    var b = 0;

                    int divide = 0;

                    for (int xx = i; xx < i + pixelSize && xx < endx; xx++)
                    {
                        for (int yy = j; yy < j + pixelSize && yy < endy; yy++)
                        {
                            var offset = ((yy * width) + xx) * depth;
                            r += buffer[offset + 0];
                            g += buffer[offset + 1];
                            b += buffer[offset + 2];
                            ++divide;
                        }
                    }

                    r /= divide;
                    g /= divide;
                    b /= divide;


                    for (int xx = i; xx < i + pixelSize && xx < endx; xx++)
                    {
                        for (int yy = j; yy < j + pixelSize && yy < endy; yy++)
                        {
                            var offset = ((yy * width) + xx) * depth;
                            buffer[offset + 0] = (byte)r;
                            buffer[offset + 1] = (byte)g;
                            buffer[offset + 2] = (byte)b;
                        }
                    }

                }
            }
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
                
                tempBitmap?.Save(_convertMemoryStream, ImageFormat.Png);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                _convertMemoryStream.Seek(0, SeekOrigin.Begin);
                image.StreamSource = _convertMemoryStream;
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
