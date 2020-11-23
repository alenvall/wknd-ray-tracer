using System;

namespace WeekendRayTracer.Models.Textures
{
    public class Perlin
    {
        private static int PointCount { get; } = 256;

        private Vec3[] RandomVectors { get; }
        private int[] PermX { get; }
        private int[] PermY { get; }
        private int[] PermZ { get; }

        public Perlin()
        {
            var rand = new Random();
            RandomVectors = new Vec3[PointCount];

            for (int i = 0; i < PointCount; i++)
            {
                RandomVectors[i] = Vec3.Random(-1, 1).Unit();
            }

            PermX = GeneratePermutation(rand);
            PermY = GeneratePermutation(rand);
            PermZ = GeneratePermutation(rand);
        }

        public float GetTurbulence(in Vec3 point, int depth = 7)
        {
            float accumulated = 0;
            float weight = 1;
            var tempPoint = point;


            var vectors = new Vec3[2, 2, 2];
            for (int i = 0; i < depth; i++)
            {
                accumulated += weight * GetNoise(ref vectors, in tempPoint);
                weight *= 0.5f;
                tempPoint *= 2;
            }

            return Math.Abs(accumulated);
        }

        private static int[] GeneratePermutation(Random rand)
        {
            var array = new int[PointCount];

            for (int i = 0; i < PointCount; i++)
            {
                array[i] = i;
            }

            Permute(ref array, PointCount, rand);

            return array;
        }

        private static void Permute(ref int[] values, int n, Random rand)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int target = rand.Next(0, i + 1);
                var temp = values[i];
                values[i] = values[target];
                values[target] = temp;
            }
        }

        private static float PerlinInterpolation(ref Vec3[,,] vectors, float u, float v, float w)
        {
            var uu = u * u * (3 - 2 * u);
            var vv = v * v * (3 - 2 * v);
            var ww = w * w * (3 - 2 * w);
            var accumulated = 0.0f;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var weight = new Vec3(u - i, v - j, w - k);
                        accumulated += (i * uu + (1 - i) * (1 - uu))
                                     * (j * vv + (1 - j) * (1 - vv))
                                     * (k * ww + (1 - k) * (1 - ww))
                                     * weight.Dot(vectors[i, j, k]);
                    }
                }
            }

            return accumulated;
        }

        private float GetNoise(ref Vec3[,,] vectors, in Vec3 point)
        {
            var u = point.X - (float)Math.Floor(point.X);
            var v = point.Y - (float)Math.Floor(point.Y);
            var w = point.Z - (float)Math.Floor(point.Z);
            var i = (int)Math.Floor(point.X);
            var j = (int)Math.Floor(point.Y);
            var k = (int)Math.Floor(point.Z);

            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        vectors[di, dj, dk] = RandomVectors[
                            PermX[(i + di) & 255] ^
                            PermY[(j + dj) & 255] ^
                            PermZ[(k + dk) & 255]];
                    }
                }
            }

            return PerlinInterpolation(ref vectors, u, v, w);
        }

    }
}
