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
            Even = new ColorTexture(color1);
            Odd = new ColorTexture(color2);
        }

        public Vec3 GetColorValue(float u, float v, in Vec3 point)
        {
            var sines = Math.Sin(10 * point.X) * Math.Sin(10 * point.Y) * Math.Sin(10 * point.Z);

            if (sines < 0)
            {
                return Odd.GetColorValue(u, v, in point);
            }
            else
            {
                return Even.GetColorValue(u, v, in point);
            }
        }
    }
}
