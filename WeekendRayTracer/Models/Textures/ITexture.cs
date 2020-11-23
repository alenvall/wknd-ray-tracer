
namespace WeekendRayTracer.Models.Textures
{
    public interface ITexture
    {
        public Vec3 GetColorValue(float u, float v, in Vec3 point);
    }
}
