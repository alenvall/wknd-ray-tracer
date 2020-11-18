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
using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public class RayTracer
    {
        private readonly Random _rand = new Random();

        public void Run()
        {
            var aspectRatio = 16.0 / 9.0;
            var imageWidth = 600;
            var imageHeight = (int)(imageWidth / aspectRatio);
            var samplesPerPixel = 200;
            var maxDepth = 50;
            var complexity = 7;
            var renderName = $"{imageWidth}x{imageHeight}_{complexity}_{samplesPerPixel}_{maxDepth}";

            Console.WriteLine("Setting up scene and camera...");
            var camera = SetupCamera(aspectRatio);
            var scene = GenerateRandomScene(complexity);

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var parallel = RenderPixels(imageWidth, imageHeight, samplesPerPixel, maxDepth, camera, scene);
            stopwatch.Stop();

            Console.WriteLine($"\nFinished in {stopwatch.Elapsed:hh\\:mm\\:ss\\:fff}\n");
            PrintFile(imageWidth, imageHeight, parallel, renderName + $" ({Math.Round(stopwatch.Elapsed.TotalSeconds)})");

            Console.WriteLine("\nDone! Press any key to exit...");
            Console.ReadKey();
        }

        private static List<Vec3> RenderPixels(int imageWidth, int imageHeight, int samples, int maxDepth, Camera camera, Scene scene)
        {
            var queue = new ConcurrentQueue<KeyValuePair<int, Vec3>>();
            var totalPixels = imageHeight * imageWidth;
            int seed = Environment.TickCount;
            var random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
            var stackInfo = new StackTrace();
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

        private static Vec3 RenderPixel(int imageWidth, int imageHeight, int samples, int maxDepth, int row, int column, ThreadLocal<Random> random, ref Camera camera, ref Scene scene)
        {
            var color = new Vec3(0, 0, 0);
            for (int s = 0; s <= samples; s++)
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

        private static Vec3 RayColor(in Ray ray, int depth, ref Scene scene)
        {
            if (depth <= 0)
            {
                return new Vec3(0, 0, 0);
            }

            HitResult hitResult = new HitResult();
            if (scene.Hit(ref hitResult, ray, 0.001f, float.PositiveInfinity))
            {
                var scatterResult = new ScatterResult();
                if (hitResult.Material.Scatter(ref scatterResult, ray, hitResult))
                {
                    return scatterResult.Attenuation * RayColor(scatterResult.ScatteredRay, depth - 1, ref scene);
                }

                return new Vec3(0, 0, 0);
            }

            var directionUnit = ray.Direction.Unit();
            var t = 0.5f * (directionUnit.Y + 1.0f);
            return (1.0f - t) * new Vec3(1.0f, 1.0f, 1.0f) + t * new Vec3(0.5f, 0.7f, 1.0f);
        }

        private Scene GenerateRandomScene(int complexity)
        {
            var objects = new List<IHittable>();

            var groundMaterial = new Lambertian(new Vec3(0.5f, 0.5f, 0.5f));
            objects.Add(new Sphere(new Vec3(0, -1000, 0), 1000, groundMaterial));

            for (int a = -complexity; a < complexity; a++)
            {
                for (int b = -complexity; b < complexity; b++)
                {
                    var chooseMaterial = _rand.NextDouble();
                    var center = new Vec3(a + 0.9f + _rand.NextFloat(), 0.2f, b + 0.9f * _rand.NextFloat());

                    if ((center - new Vec3(4, 0.2f, 0)).Length() > 0.9f)
                    {
                        IMaterial sphereMaterial;

                        if (chooseMaterial < 0.8f)
                        {
                            // Diffuse
                            var albedo = Vec3.Random() * Vec3.Random();
                            sphereMaterial = new Lambertian(albedo);
                            var center2 = center + new Vec3(0, _rand.NextFloat(0, 0.5f), 0);
                            objects.Add(new MovingSphere(center, center2, 0.0f, 1.0f, 0.2f, sphereMaterial));
                        }
                        else if (chooseMaterial < 0.95)
                        {
                            // Metal
                            var albedo = Vec3.Random(0.5f, 1);
                            var fuzz = _rand.NextFloat(0, 0.5f);
                            sphereMaterial = new Metal(albedo, fuzz);
                            objects.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                        else
                        {
                            // Glass
                            sphereMaterial = new Dielectric(1.5f);
                            objects.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                    }
                }
            }

            var material1 = new Dielectric(1.5f);
            objects.Add(new Sphere(new Vec3(0, 1, 0), 1.0f, material1));

            var material2 = new Lambertian(new Vec3(0.4f, 0.2f, 0.1f));
            objects.Add(new Sphere(new Vec3(-4, 1, 0), 1.0f, material2));

            var material3 = new Metal(new Vec3(0.7f, 0.6f, 0.5f), 0.0f);
            objects.Add(new Sphere(new Vec3(4, 1, 0), 1.0f, material3));

            return new Scene(objects);
        }

        private static Camera SetupCamera(double aspectRatio)
        {
            var lookFrom = new Vec3(13, 2, 3);
            var lookAt = new Vec3(0, 0, 0);
            var vUp = new Vec3(0, 1, 0);
            var focusDistance = 10;
            var aperture = 0.15f;

            return new Camera(lookFrom, lookAt, vUp, 20, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
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
