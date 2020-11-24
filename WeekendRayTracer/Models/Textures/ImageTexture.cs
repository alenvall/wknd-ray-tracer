using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace WeekendRayTracer.Models.Textures
{
    public readonly struct ImageTexture : ITexture
    {
        private int BytesPerPixel { get; }
        private int BytesPerScanline { get; }
        private byte[] Data { get; }
        private int Width { get; }
        private int Height { get; }

        public ImageTexture(string fileName)
        {
            // Debug color & values if image fails to load
            Data = new byte[3] { 211, 3, 252 }; 
            Width = 1;
            Height = 1;
            BytesPerPixel = 3;
            BytesPerScanline = BytesPerPixel * Width;

            try
            {
                using Stream stream = System.IO.File.Open(fileName, System.IO.FileMode.Open);
                var image = Image.FromStream(stream);
                var bitmap = new Bitmap(image);

                Width = bitmap.Width;
                Height = bitmap.Height;
                BytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                BytesPerScanline = BytesPerPixel * Width;

                var rect = new Rectangle(0, 0, Width, Height);
                var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                Data = new byte[Width * Height * BytesPerPixel];
                Marshal.Copy(data.Scan0, Data, 0, Data.Length);

                bitmap.UnlockBits(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not load texture image file \"{fileName}\""!, e);
            }
        }

        public Vec3 GetColorValue(float u, float v, in Vec3 point)
        {
            u = Math.Clamp(u, 0, 1);
            v = 1 - Math.Clamp(v, 0, 1);

            int i = (int)(u * Width);
            int j = (int)(v * Height);

            var index = j * BytesPerScanline + i * BytesPerPixel;
            var red = Data[index + 2];
            var green = Data[index + 1];
            var blue = Data[index + 0];
            var scale = 1 / 255f;

            return new Vec3(red * scale, green * scale, blue * scale);
        }

    }
}
