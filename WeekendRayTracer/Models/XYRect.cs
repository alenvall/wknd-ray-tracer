using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct XYRect : IHittable
    {
        public float X0 { get; }
        public float X1 { get; }
        public float Y0 { get; }
        public float Y1 { get; }
        public float K { get; }
        public IMaterial Material { get; }

        public XYRect(float x0, float x1, float y0, float y1, float k, IMaterial material)
        {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
            K = k;
            Material = material;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = new AABB(new Vec3(X0, Y0, K - 0.0001f), new Vec3(X1, Y1, K + 0.0001f));
            return true;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var t = (K - ray.Origin.Z) / ray.Direction.Z;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            var x = ray.Origin.X + t * ray.Direction.X;
            var y = ray.Origin.Y + t * ray.Direction.Y;
            if (x < X0 || x > X1 || y < Y0 || y > Y1)
            {
                return false;
            }

            var p = ray.At(t);
            var u = (x - X0) / (X1 - X0);
            var v = (y - Y0) / (Y1 - Y0);
            var outwardNormal = new Vec3(0, 0, 1);
            var frontFace = ray.Direction.Dot(outwardNormal) < 0;
            var faceNormal = frontFace ? outwardNormal : -outwardNormal;

            result = new HitResult(t, p, faceNormal, frontFace, Material, u, v);

            return true;
        }
    }
}
