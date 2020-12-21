using WeekendRayTracer.Models.Textures;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models.Materials
{
    public readonly struct DiffuseLight : IMaterial
    {
        private ITexture Emit { get; }

        public DiffuseLight(ITexture texture)
        {
            Emit = texture;
        }

        public DiffuseLight(Vec3 color)
        {
            Emit = new ColorTexture(color);
        }

        public bool Scatter(ref ScatterResult scatterResult, in Ray ray, in HitResult hitResult)
        {
            return false;
        }

        public Vec3 Emitted(float u, float v, in Vec3 point)
        {
            return Emit.GetColorValue(u, v, in point);
        }
    }
}
