using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public readonly struct Metal : IMaterial
    {
        private Vec3 Albedo { get; }
        private float Fuzz { get; }

        public Metal(Vec3 albedo, float fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz;
        }

        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult hitResult)
        {
            var reflected = ray.Direction.Unit().Reflect(hitResult.Normal);
            var scatteredRay = new Ray(hitResult.P, reflected + Fuzz * Vec3.RandomInUnitSphere());

            if (scatteredRay.Direction.Dot(hitResult.Normal) > 0)
            {
                scatterResult = new ScatterResult(scatteredRay, Albedo);
            }

            return true;
        }
    }
}
