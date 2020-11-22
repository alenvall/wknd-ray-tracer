using System;
using System.Numerics;
using System.Threading;
using WeekendRayTracer.Extensions;

namespace WeekendRayTracer.Models
{
    public readonly struct Vec3
    {
        private Vector3 Vector { get; }

        public float X { get => Vector.X; }
        public float Y { get => Vector.Y; }
        public float Z { get => Vector.Z; }
        public float LengthSquared() => Vector.LengthSquared();
        public float Length() => Vector.Length();

        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));
        private static Random Rand => random.Value;

        public static Vec3 operator -(Vec3 u) => new Vec3(-u.X, -u.Y, -u.Z);
        public static Vec3 operator +(Vec3 u, Vec3 v) => new Vec3(u.Vector + v.Vector);
        public static Vec3 operator -(Vec3 u, Vec3 v) => new Vec3(u.Vector - v.Vector);
        public static Vec3 operator *(Vec3 u, Vec3 v) => new Vec3(u.Vector * v.Vector);
        public static Vec3 operator /(Vec3 u, Vec3 v) => new Vec3(u.Vector / v.Vector);
        public static Vec3 operator *(Vec3 u, float t) => new Vec3(u.Vector * t);
        public static Vec3 operator *(float t, Vec3 u) => u * t;
        public static Vec3 operator /(Vec3 u, float t) => u * (1 / t);

        public Vec3(float x, float y, float z)
        {
            Vector = new Vector3(x, y, z);
        }

        private Vec3(Vector3 vector3)
        {
            Vector = new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public Vec3 Unit()
        {
            return new Vec3(Vector3.Normalize(Vector));
        }

        public float Dot(in Vec3 v)
        {
            return Vector3.Dot(Vector, v.Vector);
        }

        public Vec3 Cross(in Vec3 v)
        {
            return new Vec3(Vector3.Cross(Vector, v.Vector));
        }

        public Vec3 Reflect(in Vec3 normal)
        {
            return new Vec3(Vector3.Reflect(Vector, normal.Vector));
        }

        public Vec3 Refract(in Vec3 normal, float etaIOverEtaT)
        {
            var cosTheta = (float)Math.Min((-this).Dot(normal), 1.0);
            var rOutPerpendicular = etaIOverEtaT * (this + cosTheta * normal);
            var rOutParallel = (float)-Math.Sqrt((float)Math.Abs(1.0 - rOutPerpendicular.LengthSquared())) * normal;

            return rOutPerpendicular + rOutParallel;
        }

        public bool NearZero()
        {
            var s = 1e-8;
            return (Math.Abs(X) < s) && (Math.Abs(Y) < s) && (Math.Abs(Z) < s);
        }

        public static Vec3 Random()
        {
            return new Vec3((float)Rand.NextDouble(), (float)Rand.NextDouble(), (float)Rand.NextDouble());
        }

        public static Vec3 Random(float min, float max)
        {
            return new Vec3(Rand.NextFloat(min, max), Rand.NextFloat(min, max), Rand.NextFloat(min, max));
        }

        public static Vec3 RandomInUnitSphere()
        {
            while (true)
            {
                var p = Random(-1, 1);
                if (p.LengthSquared() < 1)
                {
                    return p;
                }
            }
        }

        public static Vec3 RandomUnitVector()
        {
            return RandomInUnitSphere().Unit();
        }

        public static Vec3 RandomInUnitDisk()
        {
            while (true)
            {
                var p = new Vec3(Rand.NextFloat(-1, 1), Rand.NextFloat(-1, 1), 0);

                if (p.LengthSquared() < 1)
                {
                    return p;
                }
            }
        }

    }
}
