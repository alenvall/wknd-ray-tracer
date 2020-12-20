using System;
using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Textures;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct ConstantDensityMedium : IHittable
    {
        public IHittable Boundary { get; }
        public IMaterial PhaseFunction { get; }
        float NegativeInverseDensity { get; }

        public ConstantDensityMedium(IHittable boundary, float density, ITexture texture)
        {
            Boundary = boundary;
            NegativeInverseDensity = -1 / density;
            PhaseFunction = new Isotropic(texture);
        }

        public ConstantDensityMedium(IHittable boundary, float density, Vec3 color)
        {
            Boundary = boundary;
            NegativeInverseDensity = -1 / density;
            PhaseFunction = new Isotropic(color);
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            return Boundary.BoundingBox(ref box, time0, time1);
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var result1 = new HitResult();
            if (!Boundary.Hit(ref result1, in ray, float.NegativeInfinity, float.PositiveInfinity))
            {
                return false;
            }

            var result2 = new HitResult();
            if (!Boundary.Hit(ref result2, in ray, result1.T + 0.0001f, float.PositiveInfinity))
            {
                return false;
            }

            var newTMin = result1.T;
            if (newTMin < tMin)
            {
                newTMin = tMin;
            }

            var newTMax = result2.T;
            if (newTMax > tMax)
            {
                newTMax = tMax;
            }

            if (newTMin > newTMax)
            {
                return false;
            }

            if (newTMin < 0)
            {
                newTMin = 0;
            }

            var rayLength = ray.Direction.Length();
            var distanceInsideBoundary = (newTMax - newTMin) * rayLength;
            var hitDistance = NegativeInverseDensity * Math.Log(StaticRandom.NextDouble());

            if (hitDistance > distanceInsideBoundary)
            {
                return false;
            }

            var t = newTMin + (float)hitDistance / rayLength;
            var p = ray.At(t);
            var normal = new Vec3(1, 0, 0);
            var frontFace = true;

            result = new HitResult(t, p, normal, frontFace, PhaseFunction, result.U, result.V);
            return true;
        }
    }
}
