using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public class Lambertian : IMaterial
    {
        private readonly Vec3 _albedo;

        public Lambertian(Vec3 albedo)
        {
            _albedo = albedo;
        }

        public ScatterResult Scatter(Ray ray, HitResult hitResult)
        {
            var scatterDirection = hitResult.Normal + Vec3.RandomUnitVector();

            if (scatterDirection.NearZero())
            {
                scatterDirection = hitResult.Normal;
            }

            var scatteredRay = new Ray(hitResult.P, scatterDirection);

            return new ScatterResult(scatteredRay, _albedo);
        }
    }
}
