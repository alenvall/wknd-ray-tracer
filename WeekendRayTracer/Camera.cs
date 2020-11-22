using System;
using System.Threading;
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
        private float Time0 { get; }
        private float Time1 { get; }
        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());
        private static Random Rand => random.Value;

        public Camera(Vec3 lookFrom, Vec3 lookAt, float verticalFovDeg, float aspectRatio, float aperture, float focusDistance, float time0, float time1)
        {
            var theta = verticalFovDeg.ToRadians();
            var h = (float)Math.Tan(theta / 2);
            var viewportHeight = 2.0f * h;
            var viewportWidth = aspectRatio * viewportHeight;
            var vUp = new Vec3(0, 1, 0);

            W = (lookFrom - lookAt).Unit();
            U = (vUp.Cross(W)).Unit();
            V = W.Cross(U);

            Origin = lookFrom;
            Horizontal = focusDistance * viewportWidth * U;
            Vertical = focusDistance * viewportHeight * V;
            LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - focusDistance * W;

            LensRadius = aperture / 2;
            Time0 = time0;
            Time1 = time1;
        }

        public static Camera StandardCamera(double aspectRatio)
        {
            var lookFrom = new Vec3(13, 2, 3);
            var lookAt = new Vec3(0, 0, 0);
            var focusDistance = 10;
            var aperture = 0.15f;

            return new Camera(lookFrom, lookAt, 20, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
        }

        public Ray GetRay(float s, float t)
        {
            var randomRay = LensRadius * Vec3.RandomInUnitDisk();
            var offset = U * randomRay.X + V * randomRay.Y;
            var direction = LowerLeftCorner + s * Horizontal + t * Vertical - Origin;

            return new Ray(Origin + offset, direction - offset, Rand.NextFloat(Time0, Time1));
        }
    }
}
