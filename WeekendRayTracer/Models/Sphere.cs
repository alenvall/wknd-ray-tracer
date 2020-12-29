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

        public static void GetSphereUV(Vec3 p, out float u, out float v)
        {
            // p: a given point on the sphere of radius one, centered at the origin.
            // u: returned value [0,1] of angle around the Y axis from X=-1.
            // v: returned value [0,1] of angle from Y=-1 to Y=+1.
            //     <1 0 0> yields <0.50 0.50>       <-1  0  0> yields <0.00 0.50>
            //     <0 1 0> yields <0.50 1.00>       < 0 -1  0> yields <0.50 0.00>
            //     <0 0 1> yields <0.25 0.50>       < 0  0 -1> yields <0.75 0.50>

            // Clamp values since they might be sligthly outside bounds due to floating point shenanigans, which would cause acos()/atan2() to return NaN
            var x = Math.Clamp(p.X, -1, 1);
            var y = Math.Clamp(p.Y, -1, 1);
            var z = Math.Clamp(p.Z, -1, 1);

            var theta = Math.Acos(-y);
            var phi = Math.Atan2(-z, x) + Math.PI;

            u = (float)(phi / (2 * Math.PI));
            v = (float)(theta / Math.PI);
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
            GetSphereUV(outwardNormal, out float u, out float v);
            result = new HitResult(T, P, faceNormal, frontFace, Material, u, v);
            return true;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = new AABB(
                Center - new Vec3(Radius, Radius, Radius),
                Center + new Vec3(Radius, Radius, Radius));

            return true;
        }

    }
}
