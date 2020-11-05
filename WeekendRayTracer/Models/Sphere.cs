using System;
using System.Collections.Generic;
using System.Text;

namespace WeekendRayTracer.Models
{
    public class Sphere : IHittable
    {
        public Vec3 Center { get; set; }
        public double Radius { get; set; }

        public Sphere() { }

        public Sphere(Vec3 center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public HitRecord Hit(Ray ray, double tMin, double tMax)
        {
            var oc = ray.Origin - Center;
            var a = ray.Direction.LengthSquared;
            var bHalf = oc.Dot(ray.Direction);
            var c = oc.LengthSquared - Radius * Radius;
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

            var record = new HitRecord
            {
                T = root
            };
            record.P = ray.At(record.T);
            var outwardNormal = (record.P - Center) / Radius;
            record.SetFaceNormal(ray, outwardNormal);

            return record;
        }
    }
}
