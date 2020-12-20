using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct XZRect : IHittable
    {
        public float X0 { get; }
        public float X1 { get; }
        public float Z0 { get; }
        public float Z1 { get; }
        public float K { get; }
        public IMaterial Material { get; }

        public XZRect(float x0, float x1, float z0, float z1, float k, IMaterial material)
        {
            X0 = x0;
            X1 = x1;
            Z0 = z0;
            Z1 = z1;
            K = k;
            Material = material;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = new AABB(new Vec3(X0, K - 0.0001f, Z0), new Vec3(X1, K + 0.0001f, Z1));
            return true;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var t = (K - ray.Origin.Y) / ray.Direction.Y;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            var x = ray.Origin.X + t * ray.Direction.X;
            var z = ray.Origin.Z + t * ray.Direction.Z;
            if (x < X0 || x > X1 || z < Z0 || z > Z1)
            {
                return false;
            }

            var p = ray.At(t);
            var u = (x - X0) / (X1 - X0);
            var v = (z - Z0) / (Z1 - Z0);
            var outwardNormal = new Vec3(0, 1, 0);
            var frontFace = ray.Direction.Dot(outwardNormal) < 0;
            var faceNormal = frontFace ? outwardNormal : -outwardNormal;

            result = new HitResult(t, p, faceNormal, frontFace, Material, u, v);

            return true;
        }
    }
}
