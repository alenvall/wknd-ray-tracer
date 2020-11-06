
namespace WeekendRayTracer.Models
{
    public class ScatterResult
    {
        public Ray ScatteredRay { get; set; }
        public Vec3 Attenuation { get; set; }

        public ScatterResult(Ray scatteredRay, Vec3 attenuation)
        {
            ScatteredRay = scatteredRay;
            Attenuation = attenuation;
        }
    }
}
