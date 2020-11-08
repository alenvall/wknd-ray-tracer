using WeekendRayTracer.Models.Materials;

namespace WeekendRayTracer.Models.Tracing
{
    public class HitResult
    {
        public Vec3 P { get; set; }
        public Vec3 Normal { get; set; }
        public double T { get; set; }
        public bool FrontFace { get; set; }
        public IMaterial Material { get; set; }

        public HitResult() { }

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
