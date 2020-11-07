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

        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vUp, double verticalFovDeg, double aspectRatio)
        {
            var theta = verticalFovDeg.ToRadians();
            var h = Math.Tan(theta / 2);
            var viewportHeight = 2.0 * h;
            var viewportWidth = aspectRatio * viewportHeight;

            var w = (lookFrom - lookAt).Unit();
            var u = (vUp.Cross(w)).Unit();
            var v = w.Cross(u);

            origin = lookFrom;
            horizontal = viewportWidth * u;
            vertical = viewportHeight * v;
            lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - w;
        }

        public Ray GetRay(double s, double t)
        {
            return new Ray(origin, lowerLeftCorner + s * horizontal + t * vertical - origin);
        }
    }
}
