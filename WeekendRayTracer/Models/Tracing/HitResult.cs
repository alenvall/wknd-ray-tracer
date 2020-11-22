using WeekendRayTracer.Models.Materials;

namespace WeekendRayTracer.Models.Tracing
{
    public readonly ref struct HitResult
    {
        public Vec3 P { get; }
        public Vec3 Normal { get; }
        public float T { get;  }
        public bool FrontFace { get; }
        public IMaterial Material { get; }
        public float U { get; }
        public float V { get; }

        public HitResult(float t, Vec3 p, Vec3 normal, bool frontFace, IMaterial material, float u, float v)
        {
            P = p;
            T = t;
            U = u;
            V = v;
            Normal = normal;
            FrontFace = frontFace;
            Material = material;
        }
    }
}
