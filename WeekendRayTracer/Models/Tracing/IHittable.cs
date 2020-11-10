
namespace WeekendRayTracer.Models.Tracing
{
    public interface IHittable
    {
        public abstract bool Hit(ref HitResult result, in Ray ray, double tMin, double tMax);
    }
}
