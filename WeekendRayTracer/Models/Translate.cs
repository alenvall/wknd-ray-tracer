using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct Translate : IHittable
    {
        public IHittable Object { get; }
        public Vec3 Offset { get; }

        public Translate(in IHittable obj, in Vec3 displacement)
        {
            Object = obj;
            Offset = displacement;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            if (Object.BoundingBox(ref box, time0, time1))
            {
                box = new AABB(box.Minimum + Offset, box.Maximum + Offset);

                return true;
            }

            return false;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var movedRay = new Ray(ray.Origin - Offset, ray.Direction, ray.Time);

            if (Object.Hit(ref result, in movedRay, tMin, tMax))
            {
                var movedP = result.P + Offset;

                result = new HitResult(result.T, movedP, result.Normal, result.FrontFace, result.Material, result.U, result.V);
                return true;
            }

            return false;
        }
    }
}
