﻿using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public readonly struct Metal : IMaterial
    {
        private Vec3 Albedo { get; }
        private float Fuzz { get; }

        public Metal(Vec3 albedo, float fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz;
        }

        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult hitResult)
        {
            var reflected = ray.Direction.Unit().Reflect(hitResult.Normal);
            var scatteredRay = new Ray(hitResult.P, reflected + Fuzz * Vec3.RandomInUnitSphere(), ray.Time);

            if (scatteredRay.Direction.Dot(hitResult.Normal) > 0)
            {
                scatterResult = new ScatterResult(scatteredRay, Albedo);
            }

            return true;
        }

        public Vec3 Emitted(float u, float v, in Vec3 point)
        {
            return new Vec3(0, 0, 0);
        }
    }
}
