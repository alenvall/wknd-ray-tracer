
namespace WeekendRayTracer.Models
{
    public interface IHittable
    {
        public abstract HitRecord Hit(Ray ray, double tMin, double tMax);
    }
}
