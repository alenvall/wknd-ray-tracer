
namespace WeekendRayTracer
{
    public class Ray
    {
        public Vec3 Origin { get; }
        public Vec3 Direction { get; }

        public Vec3 At(double t)
        {
            return Origin + Direction * t;
        }

        public Ray(Vec3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

    }
}
