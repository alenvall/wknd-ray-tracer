using System;
using System.Threading;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public readonly struct Dielectric : IMaterial
    {
        private float RefractionIndex { get; }

        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));
        private static Random Rand => random.Value;

        public Dielectric(float refractionIndex)
        {
            this.RefractionIndex = refractionIndex;
        }

        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult result)
        {
            float refractionRatio = result.FrontFace ? (1.0f / RefractionIndex) : RefractionIndex;
            var directionUnit = ray.Direction.Unit();

            double cosTheta = Math.Min((-directionUnit).Dot(result.Normal), 1.0);
            double sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

            var cannotRefract = refractionRatio * sinTheta > 1.0;

            Vec3 direction;
            if (cannotRefract || Reflectance(cosTheta, refractionRatio) > Rand.NextDouble())
            {
                direction = directionUnit.Reflect(result.Normal);
            }
            else
            {
                direction = directionUnit.Refract(result.Normal, refractionRatio);
            }

            scatterResult = new ScatterResult(new Ray(result.P, direction), new Vec3(1.0f, 1.0f, 1.0f));

            return true;
        }

        private static double Reflectance(double cosine, double refractionRatio)
        {
            // Schlick's approximation 
            var r0 = (1 - refractionRatio) / (1 + refractionRatio);
            r0 *= r0;

            return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
        }
    }
}
