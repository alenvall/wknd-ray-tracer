using System;
using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public class Sphere : IHittable
    {
        private readonly Vec3 _center;
        private readonly double _radius;
        private readonly IMaterial _material;

        public Sphere(Vec3 center, double radius, IMaterial material)
        {
            _center = center;
            _radius = radius;
            _material = material;
        }

        public HitResult Hit(Ray ray, double tMin, double tMax)
        {
            var oc = ray.Origin - _center;
            var a = ray.Direction.LengthSquared();
            var bHalf = oc.Dot(ray.Direction);
            var c = oc.LengthSquared() - _radius * _radius;
            var discriminant = bHalf * bHalf - a * c;

            if (discriminant < 0)
            {
                return null;
            }

            var squared = Math.Sqrt(discriminant);

            // Check boundaries
            var root = (-bHalf - squared) / a;
            if (root < tMin || tMax < root)
            {
                root = (-bHalf + squared) / a;

                if (root < tMin || tMax < root)
                {
                    return null;

                }
            }

            var T = root;
            var P = ray.At(T);
            var outwardNormal = (P - _center) / _radius;
            var frontFace = ray.Direction.Dot(outwardNormal) < 0;
            var faceNormal = frontFace ? outwardNormal : -outwardNormal;

            return new HitResult(T, P, faceNormal, frontFace, _material);
        }
    }
}
