
namespace WeekendRayTracer.Models
{
    public class HitRecord
    {
        public Vec3 P { get; set; }
        public Vec3 Normal { get; set; }
        public double T { get; set; }
        public bool FrontFace { get; set; }

        public HitRecord() { }

        public void SetFaceNormal(Ray ray, Vec3 outwardNormal)
        {
            FrontFace = ray.Direction.Dot(outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}
