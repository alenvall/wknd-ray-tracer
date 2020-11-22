using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public readonly struct Lambertian : IMaterial
    {
        private Vec3 Albedo { get; }

        public Lambertian(Vec3 albedo)
        {
            Albedo = albedo;
        }

        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult hitResult)
        {
            var scatterDirection = hitResult.Normal + Vec3.RandomUnitVector();

            if (scatterDirection.NearZero())
            {
                scatterDirection = hitResult.Normal;
            }

            var scatteredRay = new Ray(hitResult.P, scatterDirection, ray.Time);
            scatterResult = new ScatterResult(scatteredRay, Albedo);

            return true;
        }
    }
}
