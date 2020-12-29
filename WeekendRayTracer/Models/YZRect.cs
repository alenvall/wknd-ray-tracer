using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct YZRect : IHittable
    {
        public float Y0 { get; }
        public float Y1 { get; }
        public float Z0 { get; }
        public float Z1 { get; }
        public float K { get; }
        public IMaterial Material { get; }

        public YZRect(float y0, float y1, float z0, float z1, float k, IMaterial material)
        {
            Y0 = y0;
            Y1 = y1;
            Z0 = z0;
            Z1 = z1;
            K = k;
            Material = material;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = new AABB(new Vec3(K - 0.0001f, Y0, Z0), new Vec3(K + 0.0001f, Y1, Z1));
            return true;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var t = (K - ray.Origin.X) / ray.Direction.X;
            if (float.IsNaN(t) || float.IsInfinity(t))
            {
                // Hack to avoid dividing by zero and causing NaN to get passed along or 
                // dividing zero by zero and passing Infinity along
                return false;
            }

            if (t < tMin || t > tMax)
            {
                return false;
            }

            var y = ray.Origin.Y + t * ray.Direction.Y;
            var z = ray.Origin.Z + t * ray.Direction.Z;
            if (y < Y0 || y > Y1 || z < Z0 || z > Z1)
            {
                return false;
            }

            var p = ray.At(t);
            var u = (y - Y0) / (Y1 - Y0);
            var v = (z - Z0) / (Z1 - Z0);
            var outwardNormal = new Vec3(1, 0, 0);
            var frontFace = ray.Direction.Dot(outwardNormal) < 0;
            var faceNormal = frontFace ? outwardNormal : -outwardNormal;

            result = new HitResult(t, p, faceNormal, frontFace, Material, u, v);

            return true;
        }
    }
}
