using System;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models;

namespace WeekendRayTracer
{
    public class Camera
    {
        private readonly Vec3 origin;
        private readonly Vec3 lowerLeftCorner;
        private readonly Vec3 horizontal;
        private readonly Vec3 vertical;
        private readonly Vec3 w;
        private readonly Vec3 u;
        private readonly Vec3 v;
        private readonly double lensRadius;

        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vUp, double verticalFovDeg, double aspectRatio, double aperture, double focusDistance)
        {
            var theta = verticalFovDeg.ToRadians();
            var h = Math.Tan(theta / 2);
            var viewportHeight = 2.0 * h;
            var viewportWidth = aspectRatio * viewportHeight;

            w = (lookFrom - lookAt).Unit();
            u = (vUp.Cross(w)).Unit();
            v = w.Cross(u);

            origin = lookFrom;
            horizontal = focusDistance * viewportWidth * u;
            vertical = focusDistance * viewportHeight * v;
            lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - focusDistance * w;

            lensRadius = aperture / 2;
        }

        public Ray GetRay(double s, double t)
        {
            var randomRay = lensRadius * Vec3.RandomInUnitDisk();
            var offset = u * randomRay.X + v * randomRay.Y;
            var direction = lowerLeftCorner + s * horizontal + t * vertical - origin;

            return new Ray(origin + offset, direction - offset);
        }
    }
}
