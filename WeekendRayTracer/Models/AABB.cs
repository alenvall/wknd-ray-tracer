using System;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct AABB : IHittable
    {
        public Vec3 Minimum { get; }
        public Vec3 Maximum { get; }

        public AABB(Vec3 minimum, Vec3 maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public static AABB SurroundingBox(in AABB box0, in AABB box1)
        {
            var minimum = new Vec3(
                Math.Min(box0.Minimum.X, box1.Minimum.X),
                Math.Min(box0.Minimum.Y, box1.Minimum.Y),
                Math.Min(box0.Minimum.Z, box1.Minimum.Z));

            var maximum = new Vec3(
                Math.Max(box0.Maximum.X, box1.Maximum.X),
                Math.Max(box0.Maximum.Y, box1.Maximum.Y),
                Math.Max(box0.Maximum.Z, box1.Maximum.Z));

            return new AABB(minimum, maximum);
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = new AABB(Minimum, Maximum);
            return true;
        }

        public bool Hit(ref HitResult hitResult, in Ray ray, float tMin, float tMax)
        {
            // Check X
            var invD = 1.0f / ray.Direction.X;
            var t0 = (Minimum.X - ray.Origin.X) * invD;
            var t1 = (Maximum.X - ray.Origin.X) * invD;

            if (invD < 0.0f)
            {
                var temp = t0;
                t0 = t1;
                t1 = temp;
            }

            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;

            if (tMax <= tMin)
            {
                return false;
            }

            // Check Y
            invD = 1.0f / ray.Direction.Y;
            t0 = (Minimum.Y - ray.Origin.Y) * invD;
            t1 = (Maximum.Y - ray.Origin.Y) * invD;

            if (invD < 0)
            {
                var temp = t0;
                t0 = t1;
                t1 = temp;
            }

            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;

            if (tMax <= tMin)
            {
                return false;
            }

            // Check Z
            invD = 1.0f / ray.Direction.Z;
            t0 = (Minimum.Z - ray.Origin.Z) * invD;
            t1 = (Maximum.Z - ray.Origin.Z) * invD;

            if (invD < 0)
            {
                var temp = t0;
                t0 = t1;
                t1 = temp;
            }

            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;

            if (tMax <= tMin)
            {
                return false;
            }

            // Hit
            return true;
        }

    }
}
