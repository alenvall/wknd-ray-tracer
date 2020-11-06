
namespace WeekendRayTracer.Models
{
    public class Metal : IMaterial
    {
        public Vec3 Albedo { get; set; }
        public double Fuzz { get; set; }

        public Metal(Vec3 albedo, double fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz;
        }

        public ScatterResult Scatter(Ray ray, HitRecord record)
        {
            var reflected = ray.Direction.Unit().Reflect(record.Normal);
            var scatteredRay = new Ray(record.P, reflected + Fuzz * Vec3.RandomInUnitSphere());

            if (scatteredRay.Direction.Dot(record.Normal) > 0)
            {
                return new ScatterResult(scatteredRay, Albedo);
            }

            return null;
        }
    }
}
