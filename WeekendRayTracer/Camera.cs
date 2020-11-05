using WeekendRayTracer.Models;

namespace WeekendRayTracer
{
    public class Camera
    {
        private readonly Vec3 origin;
        private readonly Vec3 lowerLeftCorner;
        private readonly Vec3 horizontal;
        private readonly Vec3 vertical;

        public Camera()
        {
            var aspectRatio = 16.0 / 9.0;
            var viewportHeight = 2.0;
            var viewportWidth = aspectRatio * viewportHeight;
            var focalLength = 1.0;

            origin = new Vec3(0, 0, 0);
            horizontal = new Vec3(viewportWidth, 0, 0);
            vertical = new Vec3(0, viewportHeight, 0);
            lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - new Vec3(0, 0, focalLength);
        }

        public Ray GetRay(double u, double v)
        {
            return new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
        }
    }
}
