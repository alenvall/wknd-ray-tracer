
namespace WeekendRayTracer.Models
{
    public class Lambertian : IMaterial
    {
        public Vec3 Albedo { get; set; }

        public Lambertian(Vec3 albedo)
        {
            Albedo = albedo;
        }

        public ScatterResult Scatter(Ray ray, HitRecord record)
        {
            var scatterDirection = record.Normal + Vec3.RandomUnitVector();

            if (scatterDirection.NearZero())
            {
                scatterDirection = record.Normal;
            }

            return new ScatterResult(new Ray(record.P, scatterDirection), Albedo);
        }
    }
}
