using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct Box : IHittable
    {
        public Vec3 BoxMin { get; }
        public Vec3 BoxMax { get; }
        public Scene Sides { get; }

        public Box(Vec3 p0, Vec3 p1, IMaterial material)
        {
            BoxMin = p0;
            BoxMax = p1;

            var scene = new Scene();
            scene.Add(new XYRect(p0.X, p1.X, p0.Y, p1.Y, p1.Z, material));
            scene.Add(new XYRect(p0.X, p1.X, p0.Y, p1.Y, p0.Z, material));
            scene.Add(new XZRect(p0.X, p1.X, p0.Z, p1.Z, p1.Y, material));
            scene.Add(new XZRect(p0.X, p1.X, p0.Z, p1.Z, p0.Y, material));
            scene.Add(new YZRect(p0.Y, p1.Y, p0.Z, p1.Z, p1.X, material));
            scene.Add(new YZRect(p0.Y, p1.Y, p0.Z, p1.Z, p0.X, material));

            Sides = scene;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            return Sides.Hit(ref result, ray, tMin, tMax);
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = new AABB(BoxMin, BoxMax);

            return true;
        }
    }
}
