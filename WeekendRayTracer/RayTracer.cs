using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private Scene _scene;
        private Camera _camera;
        private readonly Random _rand = new Random();

        public void Run()
        {
            var aspectRatio = 16.0 / 9.0;
            var imageWidth = 600;
            var imageHeight = (int)(imageWidth / aspectRatio);
            var samplesPerPixel = 200;
            var maxDepth = 50;
            var complexity = 4;
            var renderName = $"{imageWidth}x{imageHeight}_{complexity}_{samplesPerPixel}_{maxDepth}";

            Console.WriteLine("Setting up scene and camera...");
            _camera = SetupCamera(aspectRatio);
            _scene = GenerateRandomScene(complexity);

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var parallel = RenderPixelsParallel(imageWidth, imageHeight, samplesPerPixel, maxDepth);
            stopwatch.Stop();

            Console.WriteLine($"\nParallel finished in {stopwatch.Elapsed:hh\\:mm\\:ss\\:fff}\n");
            PrintFile(imageWidth, imageHeight, parallel, renderName + $"_para ({Math.Round(stopwatch.Elapsed.TotalMinutes)})");

            //stopwatch.Restart();
            //var sequential = RenderPixelsSequential(imageWidth, imageHeight, samplesPerPixel, maxDepth);
            //stopwatch.Stop();

            //Console.WriteLine($"\nSequential finished in {stopwatch.Elapsed:hh\\:mm\\:ss\\:fff}\n");
            //PrintFile(imageWidth, imageHeight, sequential, renderName + $"_seq ({Math.Round(stopwatch.Elapsed.TotalMinutes)})");

            Console.WriteLine("\nDone! Press any key to exit...");
            Console.ReadKey();
        }

        private List<Vec3> RenderPixelsParallel(int imageWidth, int imageHeight, int samples, int maxDepth)
        {
            var queue = new ConcurrentQueue<KeyValuePair<int, Vec3>>();
            var totalPixels = imageHeight * imageWidth;
            int seed = Environment.TickCount;

            Console.Write("Rendering scene... 0%");
            Parallel.For(1, imageHeight + 1, row =>
            {
                var random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

                for (int column = 1; column < imageWidth + 1; column++)
                {
                    var color = new Vec3(0, 0, 0);
                    for (int s = 0; s < samples; ++s)
                    {
                        var j = imageHeight - row;
                        var i = column - 1;
                        var u = (i + random.Value.NextDouble()) / (imageWidth - 1);
                        var v = (j + random.Value.NextDouble()) / (imageHeight - 1);
                        var ray = _camera.GetRay(u, v);
                        color += RayColor(ray, maxDepth);
                    }

                    var scale = 1.0 / samples;
                    var red = Math.Sqrt(scale * color.X);
                    var green = Math.Sqrt(scale * color.Y);
                    var blue = Math.Sqrt(scale * color.Z);

                    int clampedRed = (int)(256 * Math.Clamp(red, 0.0, 0.999));
                    int clampedGreen = (int)(256 * Math.Clamp(green, 0.0, 0.999));
                    int clampedBlue = (int)(256 * Math.Clamp(blue, 0.0, 0.999));

                    var pixel = new Vec3(clampedRed, clampedGreen, clampedBlue);

                    var index = (row - 1) * imageWidth + column;
                    queue.Enqueue(new KeyValuePair<int, Vec3>(index, pixel));
                }

                Console.Write("\rRendering scene... {0}% ", Math.Round((double)100 * queue.Count / totalPixels));
            });

            return queue.ToArray().OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
        }

        private List<Vec3> RenderPixelsSequential(int imageWidth, int imageHeight, int samples, int maxDepth)
        {
            var pixels = new List<Vec3>();
            var totalPixels = imageHeight * imageWidth;

            Console.Write("Rendering scene... 0%");
            for (int j = imageHeight - 1; j >= 0; --j)
            {
                for (int i = 0; i < imageWidth; ++i)
                {
                    var color = new Vec3(0, 0, 0);
                    for (int s = 0; s < samples; ++s)
                    {
                        var u = (i + _rand.NextDouble()) / (imageWidth - 1);
                        var v = (j + _rand.NextDouble()) / (imageHeight - 1);
                        var ray = _camera.GetRay(u, v);
                        color += RayColor(ray, maxDepth);
                    }

                    var scale = 1.0 / samples;
                    var red = Math.Sqrt(scale * color.X);
                    var green = Math.Sqrt(scale * color.Y);
                    var blue = Math.Sqrt(scale * color.Z);

                    int clampedRed = (int)(256 * Math.Clamp(red, 0.0, 0.999));
                    int clampedGreen = (int)(256 * Math.Clamp(green, 0.0, 0.999));
                    int clampedBlue = (int)(256 * Math.Clamp(blue, 0.0, 0.999));

                    pixels.Add(new Vec3(clampedRed, clampedGreen, clampedBlue));
                }

                Console.Write("\rRendering scene... {0}% ", Math.Round((double)100 * pixels.Count / totalPixels));
            }

            return pixels;
        }

        private Vec3 RayColor(Ray ray, int depth)
        {
            if (depth <= 0)
            {
                return new Vec3(0, 0, 0);
            }

            var hitResult = _scene.Hit(ray, 0.001, double.PositiveInfinity);
            if (hitResult != null)
            {
                var scatterResult = hitResult.Material.Scatter(ray, hitResult);
                if (scatterResult != null)
                {
                    return scatterResult.Attenuation * RayColor(scatterResult.ScatteredRay, depth - 1);
                }

                return new Vec3(0, 0, 0);
            }

            var directionUnit = ray.Direction.Unit();
            var t = 0.5 * (directionUnit.Y + 1.0);
            return (1.0 - t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0);
        }

        private Scene GenerateRandomScene(int complexity)
        {
            var scene = new Scene();

            var groundMaterial = new Lambertian(new Vec3(0.5, 0.5, 0.5));
            scene.Add(new Sphere(new Vec3(0, -1000, 0), 1000, groundMaterial));

            for (int a = -complexity; a < complexity; a++)
            {
                for (int b = -complexity; b < complexity; b++)
                {
                    var chooseMaterial = _rand.NextDouble();
                    var center = new Vec3(a + 0.9 + _rand.NextDouble(), 0.2, b + 0.9 * _rand.NextDouble());

                    if ((center - new Vec3(4, 0.2, 0)).Length() > 0.9)
                    {
                        IMaterial sphereMaterial;

                        if (chooseMaterial < 0.8)
                        {
                            // Diffuse
                            var albedo = Vec3.Random() * Vec3.Random();
                            sphereMaterial = new Lambertian(albedo);
                            scene.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                        else if (chooseMaterial < 0.95)
                        {
                            // Metal
                            var albedo = Vec3.Random(0.5, 1);
                            var fuzz = _rand.NextDouble(0, 0.5);
                            sphereMaterial = new Metal(albedo, fuzz);
                            scene.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                        else
                        {
                            // Glass
                            sphereMaterial = new Dielectric(1.5);
                            scene.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                    }
                }
            }

            var material1 = new Dielectric(1.5);
            scene.Add(new Sphere(new Vec3(0, 1, 0), 1.0, material1));

            var material2 = new Lambertian(new Vec3(0.4, 0.2, 0.1));
            scene.Add(new Sphere(new Vec3(-4, 1, 0), 1.0, material2));

            var material3 = new Metal(new Vec3(0.7, 0.6, 0.5), 0.0);
            scene.Add(new Sphere(new Vec3(4, 1, 0), 1.0, material3));

            return scene;
        }

        private Camera SetupCamera(double aspectRatio)
        {
            var lookFrom = new Vec3(13, 2, 3);
            var lookAt = new Vec3(0, 0, 0);
            var vUp = new Vec3(0, 1, 0);
            var focusDistance = 10.0;
            var aperture = 0.1;

            return new Camera(lookFrom, lookAt, vUp, 20, aspectRatio, aperture, focusDistance);
        }

        private void PrintFile(int imageWidth, int imageHeight, List<Vec3> pixels, string renderName)
        {
            using StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), renderName + ".ppm"));
            outputFile.Write($"P3\n{imageWidth} {imageHeight} \n255\n");

            foreach (string line in pixels.Select(p => $"{p.X} {p.Y} {p.Z}\n"))
            {
                outputFile.Write(line);
            }
        }

    }
}
