using System;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public class Camera
    {
        private readonly Vec3 _origin;
        private readonly Vec3 _lowerLeftCorner;
        private readonly Vec3 _horizontal;
        private readonly Vec3 _vertical;
        private readonly Vec3 _w;
        private readonly Vec3 _u;
        private readonly Vec3 _v;
        private readonly double _lensRadius;

        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vUp, double verticalFovDeg, double aspectRatio, double aperture, double focusDistance)
        {
            var theta = verticalFovDeg.ToRadians();
            var h = Math.Tan(theta / 2);
            var viewportHeight = 2.0 * h;
            var viewportWidth = aspectRatio * viewportHeight;

            _w = (lookFrom - lookAt).Unit();
            _u = (vUp.Cross(_w)).Unit();
            _v = _w.Cross(_u);

            _origin = lookFrom;
            _horizontal = focusDistance * viewportWidth * _u;
            _vertical = focusDistance * viewportHeight * _v;
            _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - focusDistance * _w;

            _lensRadius = aperture / 2;
        }

        public Ray GetRay(double s, double t)
        {
            var randomRay = _lensRadius * Vec3.RandomInUnitDisk();
            var offset = _u * randomRay.X + _v * randomRay.Y;
            var direction = _lowerLeftCorner + s * _horizontal + t * _vertical - _origin;

            return new Ray(_origin + offset, direction - offset);
        }
    }
}
