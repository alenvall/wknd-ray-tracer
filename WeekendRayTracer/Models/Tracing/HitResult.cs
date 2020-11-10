using WeekendRayTracer.Models.Materials;

namespace WeekendRayTracer.Models.Tracing
{
    public readonly struct HitResult
    {
        public Vec3 P { get; }
        public Vec3 Normal { get; }
        public double T { get;  }
        public bool FrontFace { get; }
        public IMaterial Material { get; }

        public HitResult(double t, Vec3 p, Vec3 normal, bool frontFace, IMaterial material)
        {
            P = p;
            T = t;
            Normal = normal;
            FrontFace = frontFace;
            Material = material;
        }
    }
}
