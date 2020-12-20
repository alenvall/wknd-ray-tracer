using System;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct RotateY : IHittable
    {
        public IHittable Object { get; }
        public float SinTheta { get; }
        public float CosTheta { get; }
        public bool HasBox { get; }
        public AABB BBox { get; }

        public RotateY(in IHittable obj, float angle)
        {
            Object = obj;
            var radians = angle.ToRadians();
            CosTheta = (float)Math.Cos(radians);
            SinTheta = (float)Math.Sin(radians);

            var bbox = new AABB();
            HasBox = Object.BoundingBox(ref bbox, 0, 1);

            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var minZ = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;
            var maxZ = float.NegativeInfinity;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var x = i * bbox.Maximum.X + (1 - i) * bbox.Minimum.X;
                        var y = j * bbox.Maximum.Y + (1 - j) * bbox.Minimum.Y;
                        var z = k * bbox.Maximum.Z + (1 - k) * bbox.Minimum.Z;

                        var newX = CosTheta * x + SinTheta * z;
                        var newZ = -SinTheta * x + CosTheta * z;

                        minX = Math.Min(minX, newX);
                        minY = Math.Min(minY, y);
                        minZ = Math.Min(minZ, newZ);
                        maxX = Math.Max(maxX, newX);
                        maxY = Math.Max(maxY, y);
                        maxZ = Math.Max(maxZ, newZ);
                    }
                }
            }

            BBox = new AABB(new Vec3(minX, minY, minZ), new Vec3(maxX, maxY, maxZ));
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = BBox;
            return HasBox;
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            var newOriginX = CosTheta * ray.Origin.X - SinTheta * ray.Origin.Z;
            var newOriginZ = SinTheta * ray.Origin.X + CosTheta * ray.Origin.Z;

            var newDirectionX = ray.Direction.X * CosTheta - SinTheta * ray.Direction.Z;
            var newDirectionZ = ray.Direction.X * SinTheta + CosTheta * ray.Direction.Z;

            var rotatedRay = new Ray(new Vec3(newOriginX, ray.Origin.Y, newOriginZ), new Vec3(newDirectionX, ray.Direction.Y, newDirectionZ), ray.Time);

            if (Object.Hit(ref result, in rotatedRay, tMin, tMax))
            {
                var pX = CosTheta * result.P.X + SinTheta * result.P.Z;
                var pZ = -SinTheta * result.P.X + CosTheta * result.P.Z;
                var P = new Vec3(pX, result.P.Y, pZ);

                var normalX = CosTheta * result.Normal.X + SinTheta * result.Normal.Z;
                var normalZ = -SinTheta * result.Normal.X + CosTheta * result.Normal.Z;
                var normal = new Vec3(normalX, result.Normal.Y, normalZ);

                var frontFace = ray.Direction.Dot(normal) < 0;
                var faceNormal = frontFace ? normal : -normal;

                result = new HitResult(result.T, P, faceNormal, frontFace, result.Material, result.U, result.V);
                return true;
            }

            return false;
        }

    }
}
