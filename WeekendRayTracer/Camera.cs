using System;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public readonly struct Camera
    {
        private Vec3 Origin { get; }
        private Vec3 LowerLeftCorner { get; }
        private Vec3 Horizontal { get; }
        private Vec3 Vertical { get; }
        private Vec3 W { get; }
        private Vec3 U { get; }
        private Vec3 V { get; }
        private float LensRadius { get; }

        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vUp, float verticalFovDeg, float aspectRatio, float aperture, float focusDistance)
        {
            var theta = verticalFovDeg.ToRadians();
            var h = (float)Math.Tan(theta / 2);
            var viewportHeight = 2.0f * h;
            var viewportWidth = aspectRatio * viewportHeight;

            W = (lookFrom - lookAt).Unit();
            U = (vUp.Cross(W)).Unit();
            V = W.Cross(U);

            Origin = lookFrom;
            Horizontal = focusDistance * viewportWidth * U;
            Vertical = focusDistance * viewportHeight * V;
            LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - focusDistance * W;

            LensRadius = aperture / 2;
        }

        public Ray GetRay(float s, float t)
        {
            var randomRay = LensRadius * Vec3.RandomInUnitDisk();
            var offset = U * randomRay.X + V * randomRay.Y;
            var direction = LowerLeftCorner + s * Horizontal + t * Vertical - Origin;

            return new Ray(Origin + offset, direction - offset);
        }
    }
}
