using System;
using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct Sphere : IHittable
    {
        private Vec3 Center { get; }
        private float Radius { get; }
        private IMaterial Material { get; }

        public Sphere(Vec3 center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var oc = ray.Origin - Center;
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
            var outwardNormal = (P - Center) / Radius;
            var frontFace = ray.Direction.Dot(outwardNormal) < 0;
            var faceNormal = frontFace ? outwardNormal : -outwardNormal;
            result = new HitResult(T, P, faceNormal, frontFace, Material);

            return true;
        }
    }
}
