using System;

namespace WeekendRayTracer.Extensions
{
    public static class Extensions
    {
        public static double ToRadians(this double degrees)
        {
            return (degrees * Math.PI) / 180;
        }

    }
}
