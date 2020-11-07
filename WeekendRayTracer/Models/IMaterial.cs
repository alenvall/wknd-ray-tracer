
namespace WeekendRayTracer.Models
{
    public interface IMaterial
    {
        public ScatterResult Scatter(Ray ray, HitRecord record);
    }
}
