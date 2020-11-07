
using System;

namespace WeekendRayTracer.Models
{
    public class Dielectric : IMaterial
    {
        public double RefractionIndex { get; set; }
        private static readonly Random rand = new Random();

        public Dielectric(double refractionIndex)
        {
            RefractionIndex = refractionIndex;
        }

        public ScatterResult Scatter(Ray ray, HitRecord record)
        {
            double refractionRatio = record.FrontFace ? (1.0 / RefractionIndex) : RefractionIndex;
            var directionUnit = ray.Direction.Unit();

            double cosTheta = Math.Min((-directionUnit).Dot(record.Normal), 1.0);
            double sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

            var cannotRefract = refractionRatio * sinTheta > 1.0;

            Vec3 direction;
            if (cannotRefract || Reflectance(cosTheta, refractionRatio) > rand.NextDouble())
            {
                direction = directionUnit.Reflect(record.Normal);
            }
            else
            {
                direction = directionUnit.Refract(record.Normal, refractionRatio);
            }

            return new ScatterResult(new Ray(record.P, direction), new Vec3(1.0, 1.0, 1.0));
        }

        private double Reflectance(double cosine, double refractionIndex)
        {
            // Schlick's approximation 
            var r0 = (1 - refractionIndex) / (1 + refractionIndex);
            r0 *= r0;

            return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
        }
    }
}
