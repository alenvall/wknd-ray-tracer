
namespace WeekendRayTracer.Models.Textures
{
    public readonly struct ColorTexture : ITexture
    {
        public Vec3 Value { get; }

        public ColorTexture(Vec3 color)
        {
            Value = color;
        }

        public ColorTexture(float red, float blue, float green)
        {
            Value = new Vec3(red, blue, green);
        }

        public Vec3 GetColorValue(float u, float v, in Vec3 point)
        {
            return Value;
        }
    }
}
