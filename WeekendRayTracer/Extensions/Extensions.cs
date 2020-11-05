using System;

namespace WeekendRayTracer.Extensions
{
    public static class Extensions
    {
        public static double ToRadians(this double degrees)
        {
            return (degrees * Math.PI) / 180;
        }

        public static double NextDouble(this Random random, double min, double max)
        {
            return min + (max - min) * random.NextDouble();
        }
    }
}
