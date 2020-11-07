using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public class Metal : IMaterial
    {
        private readonly Vec3 _albedo;
        private readonly double _fuzz;

        public Metal(Vec3 albedo, double fuzz)
        {
            _albedo = albedo;
            _fuzz = fuzz;
        }

        public ScatterResult Scatter(Ray ray, HitResult result)
        {
            var reflected = ray.Direction.Unit().Reflect(result.Normal);
            var scatteredRay = new Ray(result.P, reflected + _fuzz * Vec3.RandomInUnitSphere());

            if (scatteredRay.Direction.Dot(result.Normal) > 0)
            {
                return new ScatterResult(scatteredRay, _albedo);
            }

            return null;
        }
    }
}
