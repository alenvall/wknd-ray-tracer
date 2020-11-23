using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public class RayTracer
    {
        public static void Run()
        {
            var aspectRatio = 16.0 / 9.0;
            var imageWidth = 400;
            var imageHeight = (int)(imageWidth / aspectRatio);
            var samplesPerPixel = 50;
            var maxDepth = 50;
            var complexity = 5;
            var renderName = $"{imageWidth}x{imageHeight}_{complexity}_{samplesPerPixel}_{maxDepth}";

            Console.WriteLine("Setting up scene and camera...");
            Camera camera;
            Scene scene;

            var vFov = 20;
            var focusDistance = 10;
            var aperture = 0.15f;
            var lookAt = new Vec3(0, 0, 0);
            var lookFrom = new Vec3(13, 2, 3);

            switch (2)
            {
                case 1:
                    camera = new Camera(lookFrom, lookAt, vFov, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
                    scene = Scene.TwoCheckerSpheres();
                    break;
                    break;

                default:
                    camera = Camera.StandardCamera(aspectRatio);
                    scene = Scene.RandomSphereScene(complexity);
                    break;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var image = RenderParallel(imageWidth, imageHeight, samplesPerPixel, maxDepth, camera, scene);
            //var image = RenderSequential(imageWidth, imageHeight, samplesPerPixel, maxDepth, camera, world);
            stopwatch.Stop();

            Console.WriteLine($"\nFinished in {stopwatch.Elapsed:hh\\:mm\\:ss\\:fff}\n");
            PrintFile(imageWidth, imageHeight, image, renderName + $" ({Math.Round(stopwatch.Elapsed.TotalMilliseconds)})");

            Console.WriteLine("\nDone! Press any key to exit...");
            Console.ReadKey();
        }

        private static List<Vec3> RenderParallel(int imageWidth, int imageHeight, int samples, int maxDepth, Camera camera, IHittable scene)
        {
            var queue = new ConcurrentQueue<KeyValuePair<int, Vec3>>();
            var totalPixels = imageHeight * imageWidth;
            int seed = Environment.TickCount;
            var random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

            Console.Write("Rendering scene... 0%");
            Parallel.For(1, imageHeight + 1, row =>
            {
                Parallel.For(1, imageWidth + 1, column =>
                {
                    var pixel = RenderPixel(imageWidth, imageHeight, samples, maxDepth, row, column, random, ref camera, ref scene);
                    var index = (row - 1) * imageWidth + column;

                    queue.Enqueue(new KeyValuePair<int, Vec3>(index, pixel));
                });

                Console.Write("\rRendering scene... {0}% ", Math.Round((double)100 * queue.Count / totalPixels));
            });

            return queue.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
        }

        private static List<Vec3> RenderSequential(int imageWidth, int imageHeight, int samples, int maxDepth, Camera camera, IHittable scene)
        {
            var pixels = new List<Vec3>();
            var totalPixels = imageHeight * imageWidth;
            int seed = Environment.TickCount;
            var random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

            Console.Write("Rendering scene... 0%");
            for (int row = 1; row <= imageHeight; row++)
            {
                for (int column = 1; column <= imageWidth; column++)
                {
                    pixels.Add(RenderPixel(imageWidth, imageHeight, samples, maxDepth, row, column, random, ref camera, ref scene));
                }

                Console.Write("\rRendering scene... {0}% ", Math.Round((double)100 * pixels.Count / totalPixels));
            }

            return pixels;
        }

        private static Vec3 RenderPixel(int imageWidth, int imageHeight, int samples, int maxDepth, int row, int column, ThreadLocal<Random> random, ref Camera camera, ref IHittable scene)
        {
            var color = new Vec3(0, 0, 0);
            for (int s = 1; s <= samples; s++)
            {
                var j = imageHeight - row;
                var i = column - 1;
                var u = (i + random.Value.NextFloat()) / (imageWidth - 1);
                var v = (j + random.Value.NextFloat()) / (imageHeight - 1);
                var ray = camera.GetRay(u, v);
                color += RayColor(in ray, maxDepth, ref scene);
            }

            var scale = 1.0 / samples;
            var red = Math.Sqrt(scale * color.X);
            var green = Math.Sqrt(scale * color.Y);
            var blue = Math.Sqrt(scale * color.Z);

            int clampedRed = (int)(256 * Math.Clamp(red, 0.0, 0.999));
            int clampedGreen = (int)(256 * Math.Clamp(green, 0.0, 0.999));
            int clampedBlue = (int)(256 * Math.Clamp(blue, 0.0, 0.999));

            return new Vec3(clampedRed, clampedGreen, clampedBlue);
        }

        private static Vec3 RayColor(in Ray ray, int depth, ref IHittable target)
        {
            if (depth <= 0)
            {
                return new Vec3(0, 0, 0);
            }

            HitResult hitResult = new HitResult();
            if (target.Hit(ref hitResult, ray, 0.001f, float.PositiveInfinity))
            {
                var scatterResult = new ScatterResult();
                if (hitResult.Material.Scatter(ref scatterResult, ray, hitResult))
                {
                    return scatterResult.Attenuation * RayColor(scatterResult.ScatteredRay, depth - 1, ref target);
                }

                return new Vec3(0, 0, 0);
            }

            var directionUnit = ray.Direction.Unit();
            var t = 0.5f * (directionUnit.Y + 1.0f);
            return (1.0f - t) * new Vec3(1.0f, 1.0f, 1.0f) + t * new Vec3(0.5f, 0.7f, 1.0f);
        }

        private static void PrintFile(int imageWidth, int imageHeight, List<Vec3> pixels, string renderName)
        {
            var sb = new StringBuilder($"P3\n{imageWidth} {imageHeight} \n255\n");

            foreach (var p in pixels)
            {
                sb.Append($"{p.X} {p.Y} {p.Z}\n");
            }

            using StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), renderName + ".ppm"));
            outputFile.Write(sb);
        }

    }
}
