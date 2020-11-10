using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public interface IMaterial
    {
        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult hitResult);
    }
}
