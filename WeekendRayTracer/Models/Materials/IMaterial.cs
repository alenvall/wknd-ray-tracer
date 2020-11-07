using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public interface IMaterial
    {
        public ScatterResult Scatter(Ray ray, HitResult record);
    }
}
