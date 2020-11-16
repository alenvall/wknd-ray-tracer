

namespace WeekendRayTracer.Models.Tracing
{
    public readonly ref struct ScatterResult
    {
        public Ray ScatteredRay { get; }
        public Vec3 Attenuation { get; }

        public ScatterResult(Ray scatteredRay, Vec3 attenuation)
        {
            ScatteredRay = scatteredRay;
            Attenuation = attenuation;
        }
    }
}
