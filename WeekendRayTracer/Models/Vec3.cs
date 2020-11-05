using System;
using WeekendRayTracer.Extensions;

namespace WeekendRayTracer.Models
{
    public class Vec3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        private static Random rand = new Random();

        public static Vec3 operator -(Vec3 u) => new Vec3(-u.X, -u.Y, -u.Z);
        public static Vec3 operator +(Vec3 u, Vec3 v) => new Vec3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
        public static Vec3 operator -(Vec3 u, Vec3 v) => new Vec3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
        public static Vec3 operator *(Vec3 u, Vec3 v) => new Vec3(u.X * v.X, u.Y * v.Y, u.Z * v.Z);
        public static Vec3 operator *(Vec3 u, double t) => new Vec3(u.X * t, u.Y * t, u.Z * t);
        public static Vec3 operator *(double t, Vec3 u) => u * t;
        public static Vec3 operator /(Vec3 u, double t) => u * (1 / t);

        public double LengthSquared => X * X + Y * Y + Z * Z;
        public double Length => Math.Sqrt(LengthSquared);
        public Vec3 Unit => new Vec3(X, Y, Z) / Length;

        public double Dot(Vec3 v) => X * v.X + Y * v.Y + Z * v.Z;
        public Vec3 Cross(Vec3 v) => new Vec3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        public override string ToString() => $"{X}, {Y}, {Z}";


        public Vec3()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }

        public Vec3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vec3 Random()
        {
            return new Vec3(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
        }

        public static Vec3 Random(double min, double max)
        {
            return new Vec3(rand.NextDouble(min, max), rand.NextDouble(min, max), rand.NextDouble(min, max));
        }

        public static Vec3 RandomInUnitSphere()
        {
            while (true)
            {
                var p = Vec3.Random();
                if (p.LengthSquared < 1)
                {
                    return p;
                }
            }
        }

        public static Vec3 RandomUnitVector()
        {
            return RandomInUnitSphere().Unit;
        }
    }
}
