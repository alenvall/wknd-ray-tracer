using WeekendRayTracer.Models.Textures;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public readonly struct Isotropic : IMaterial
    {
        public ITexture Albedo { get; }

        public Isotropic(Vec3 color)
        {
            Albedo = new ColorTexture(color);
        }

        public Isotropic(ITexture texture)
        {
            Albedo = texture;
        }

        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult hitResult)
        {
            var scatteredRay = new Ray(hitResult.P, Vec3.RandomInUnitSphere(), ray.Time);
            var attenuation = Albedo.GetColorValue(hitResult.U, hitResult.V, hitResult.P);
            scatterResult = new ScatterResult(scatteredRay, attenuation);

            return true;
        }

        public Vec3 Emitted(float u, float v, in Vec3 point)
        {
            return new Vec3(0, 0, 0);
        }
    }
}
