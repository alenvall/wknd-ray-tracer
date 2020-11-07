
namespace WeekendRayTracer.Models.Tracing
{
    public interface IHittable
    {
        public abstract HitResult Hit(Ray ray, double tMin, double tMax);
    }
}
