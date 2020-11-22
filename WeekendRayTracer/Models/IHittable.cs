using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public interface IHittable
    {
        public abstract bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax);
        public abstract bool BoundingBox(ref AABB box, float time0, float time1);
    }
}
