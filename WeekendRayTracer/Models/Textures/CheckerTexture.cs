using System;

namespace WeekendRayTracer.Models.Textures
{
    public readonly struct CheckerTexture : ITexture
    {
        public ITexture Odd { get; }
        public ITexture Even { get; }

        public CheckerTexture(ITexture even, ITexture odd)
        {
            Odd = odd;
            Even = even;
        }

        public CheckerTexture(Vec3 color1, Vec3 color2)
        {
            Even = new SolidColor(color1);
            Odd = new SolidColor(color2);
        }

        public Vec3 GetColorValue(float u, float v, Vec3 point)
        {
            var sines = Math.Sin(10 * point.X) * Math.Sin(10 * point.Y) * Math.Sin(10 * point.Z);

            if (sines < 0)
            {
                return Odd.GetColorValue(u, v, point);
            }
            else
            {
                return Even.GetColorValue(u, v, point);
            }
        }
    }
}
