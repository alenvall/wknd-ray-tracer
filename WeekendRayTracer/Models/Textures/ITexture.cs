
namespace WeekendRayTracer.Models.Textures
{
    public interface ITexture
    {
        public Vec3 GetColorValue(float u, float v, Vec3 point);
    }
}
