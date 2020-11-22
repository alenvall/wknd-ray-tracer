
namespace WeekendRayTracer.Models.Tracing
{
    public readonly struct Ray
    {
        public Vec3 Origin { get; }
        public Vec3 Direction { get; }
        public float Time { get; }

        public Ray(Vec3 origin, Vec3 direction, float time)
        {
            Origin = origin;
            Direction = direction;
            Time = time;
        }

        public Vec3 At(float t)
        {
            return Origin + Direction * t;
        }

    }
}
