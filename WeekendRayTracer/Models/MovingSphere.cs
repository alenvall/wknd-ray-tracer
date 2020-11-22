using System;
using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct MovingSphere : IHittable
    {
        private Vec3 Center0 { get; }
        private Vec3 Center1 { get; }
        private float Time0 { get; }
        private float Time1 { get; }
        private float Radius { get; }
        private IMaterial Material { get; }

        public MovingSphere(Vec3 center0, Vec3 center1, float time0, float time1, float radius, IMaterial material)
        {
            Center0 = center0;
            Center1 = center1;
            Time0 = time0;
            Time1 = time1;
            Radius = radius;
            Material = material;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var oc = ray.Origin - Center(ray.Time);
            var a = ray.Direction.LengthSquared();
            var bHalf = oc.Dot(ray.Direction);
            var c = oc.LengthSquared() - Radius * Radius;
            var discriminant = bHalf * bHalf - a * c;

            if (discriminant < 0)
            {
                return false;
            }

            var squared = Math.Sqrt(discriminant);

            // Check boundaries
            var root = (-bHalf - squared) / a;
            if (root < tMin || tMax < root)
            {
                root = (-bHalf + squared) / a;

                if (root < tMin || tMax < root)
                {
                    return false;
                }
            }

            var T = (float)root;
            var P = ray.At(T);
            var outwardNormal = (P - Center(ray.Time)) / Radius;
            var frontFace = ray.Direction.Dot(outwardNormal) < 0;
            var faceNormal = frontFace ? outwardNormal : -outwardNormal;
            result = new HitResult(T, P, faceNormal, frontFace, Material, 0, 0);

            return true;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            var box0 = new AABB(
                Center(time0) - new Vec3(Radius, Radius, Radius),
                Center(time0) + new Vec3(Radius, Radius, Radius));

            var box1 = new AABB(
                Center(time1) - new Vec3(Radius, Radius, Radius),
                Center(time1) + new Vec3(Radius, Radius, Radius));

            box = AABB.SurroundingBox(box0, box1);

            return true;
        }

        private Vec3 Center(float time)
        {
            return Center0 + ((time - Time0) / (Time1 - Time0)) * (Center1 - Center0);
        }

    }
}