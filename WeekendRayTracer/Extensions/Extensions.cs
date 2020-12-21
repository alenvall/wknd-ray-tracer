using System;

namespace WeekendRayTracer.Extensions
{
    public static class Extensions
    {
        public static float ToRadians(this float degrees)
        {
            return (degrees * (float)Math.PI) / 180;
        }

    }
}
