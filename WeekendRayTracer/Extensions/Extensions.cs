using System;

namespace WeekendRayTracer.Extensions
{
    public static class Extensions
    {
        public static float ToRadians(this float degrees)
        {
            return (degrees * (float)Math.PI) / 180;
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            return min + (max - min) * (float)random.NextDouble();
        }

    }
}
