
namespace WeekendRayTracer.Models.Textures
{
    public readonly struct SolidColor : ITexture
    {
        public Vec3 Value { get; }

        public SolidColor(Vec3 color)
        {
            Value = color;
        }

        public SolidColor(float red, float blue, float green)
        {
            Value = new Vec3(red, blue, green);
        }

        public Vec3 GetColorValue(float u, float v, Vec3 point)
        {
            return Value;
        }
    }
}
