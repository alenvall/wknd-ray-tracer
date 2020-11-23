using System;

namespace WeekendRayTracer.Models.Textures
{
    public class NoiseTexture : ITexture
    {
        private Perlin Noise { get; }
        private float Scale { get; }

        public NoiseTexture(float scale)
        {
            Noise = new Perlin();
            Scale = scale;
        }

        public Vec3 GetColorValue(float u, float v, in Vec3 point)
        {
            return new Vec3(1, 1, 1) * 0.5f * (1 + (float)Math.Sin(Scale * point.Z + 10 * Noise.GetTurbulence(point)));
        }
    }
}
