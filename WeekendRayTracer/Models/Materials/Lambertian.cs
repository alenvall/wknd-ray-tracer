using WeekendRayTracer.Models.Textures;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public readonly struct Lambertian : IMaterial
    {
        private ITexture Albedo { get; }

        public Lambertian(Vec3 albedo)
        {
            Albedo = new ColorTexture(albedo);
        }

        public Lambertian(ITexture texture)
        {
            Albedo = texture;
        }

        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult hitResult)
        {
            var scatterDirection = hitResult.Normal + Vec3.RandomUnitVector();

            if (scatterDirection.NearZero())
            {
                scatterDirection = hitResult.Normal;
            }

            var scatteredRay = new Ray(hitResult.P, scatterDirection, ray.Time);
            var attenuation = Albedo.GetColorValue(hitResult.U, hitResult.V, hitResult.P);

            scatterResult = new ScatterResult(scatteredRay, attenuation);

            return true;
        }
    }
}
