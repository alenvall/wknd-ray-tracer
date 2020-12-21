using System;
using System.Threading;

namespace WeekendRayTracer
{
    public static class StaticRandom
    {
        static int seed = Environment.TickCount;
        static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Next()
        {
            return random.Value.Next();
        }
        public static int Next(int min, int max)
        {
            return random.Value.Next(min, max);
        }

        public static double NextDouble()
        {
            return random.Value.NextDouble();
        }

        public static float NextFloat()
        {
            return (float)random.Value.NextDouble();
        }

        public static float NextFloat(float min, float max)
        {
            return min + (max - min) * (float)random.Value.NextDouble();
        }
    }
}
