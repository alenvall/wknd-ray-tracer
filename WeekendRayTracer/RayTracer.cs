using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeekendRayTracer.Models;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public class RayTracer
    {
        public static void Run()
        {
            //var aspectRatio = 16.0 / 9.0;
            var aspectRatio = 1;
            var imageWidth = 600;
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
            var aperture = 0f;
            var lookAt = new Vec3(0, 0, 0);
            var lookFrom = new Vec3(13, 2, 3);
            var background = new Vec3(0.70f, 0.80f, 1.00f);

            switch (6)
            {
                case 1:
                    camera = new Camera(lookFrom, lookAt, vFov, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
                    scene = Scene.TwoCheckerSpheres();
                    break;

                case 2:
                    camera = new Camera(lookFrom, lookAt, vFov, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
                    scene = Scene.TwoPerlinSpheres();
                    break;

                case 3:
                    camera = new Camera(lookFrom, lookAt, vFov, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
                    scene = Scene.EarthSphere();
                    break;

                case 4:
                    lookFrom = new Vec3(26, 3, 6);
                    lookAt = new Vec3(0, 2, 0);
                    background = new Vec3(0, 0, 0);
                    camera = new Camera(lookFrom, lookAt, vFov, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
                    scene = Scene.SimpleLight();
                    break;

                case 5:
                    lookFrom = new Vec3(278, 278, -800);
                    lookAt = new Vec3(278, 278, 0);
                    background = new Vec3(0, 0, 0);
                    vFov = 40;
                    camera = new Camera(lookFrom, lookAt, vFov, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
                    scene = Scene.CornellBox();
                    break;

                case 6:
                    lookFrom = new Vec3(278, 278, -800);
                    lookAt = new Vec3(278, 278, 0);
                    background = new Vec3(0, 0, 0);
                    vFov = 40;
                    camera = new Camera(lookFrom, lookAt, vFov, (float)aspectRatio, aperture, focusDistance, 0.0f, 1.0f);
                    scene = Scene.CornellSmoke();
                    break;

                default:
                    aperture = 0.15f;
                    camera = Camera.StandardCamera(aspectRatio);
                    scene = Scene.RandomSphereScene(complexity);
                    break;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var image = RenderParallel(imageWidth, imageHeight, samplesPerPixel, maxDepth, camera, scene, background);
            //var image = RenderSequential(imageWidth, imageHeight, samplesPerPixel, maxDepth, camera, scene, background);
            stopwatch.Stop();

            Console.WriteLine($"\nFinished in {stopwatch.Elapsed:hh\\:mm\\:ss\\:fff}\n");
            PrintFile(imageWidth, imageHeight, image, renderName + $" ({Math.Round(stopwatch.Elapsed.TotalMilliseconds)})");

            Console.WriteLine("\nDone! Press any key to exit...");
            Console.ReadKey();
        }

        private static List<Vec3> RenderParallel(int imageWidth, int imageHeight, int samples, int maxDepth, Camera camera, IHittable scene, Vec3 background)
        {
            var queue = new ConcurrentQueue<KeyValuePair<int, Vec3>>();
            var totalPixels = imageHeight * imageWidth;

            Console.Write("Rendering scene... 0%");
            Parallel.For(1, imageHeight + 1, row =>
            {
                Parallel.For(1, imageWidth + 1, column =>
                {
                    var pixel = RenderPixel(imageWidth, imageHeight, samples, maxDepth, row, column, ref camera, ref scene, in background);
                    var index = (row - 1) * imageWidth + column;

                    queue.Enqueue(new KeyValuePair<int, Vec3>(index, pixel));
                });

                Console.Write("\rRendering scene... {0}% ", Math.Round((double)100 * queue.Count / totalPixels));
            });

            return queue.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
        }

        private static List<Vec3> RenderSequential(int imageWidth, int imageHeight, int samples, int maxDepth, Camera camera, IHittable scene, Vec3 background)
        {
            var pixels = new List<Vec3>();
            var totalPixels = imageHeight * imageWidth;

            Console.Write("Rendering scene... 0%");
            for (int row = 1; row <= imageHeight; row++)
            {
                for (int column = 1; column <= imageWidth; column++)
                {
                    pixels.Add(RenderPixel(imageWidth, imageHeight, samples, maxDepth, row, column, ref camera, ref scene, background));
                }

                Console.Write("\rRendering scene... {0}% ", Math.Round((double)100 * pixels.Count / totalPixels));
            }

            return pixels;
        }

        private static Vec3 RenderPixel(int imageWidth, int imageHeight, int samples, int maxDepth, int row, int column, ref Camera camera, ref IHittable scene, in Vec3 background)
        {
            var color = new Vec3(0, 0, 0);
            for (int s = 1; s <= samples; s++)
            {
                var j = imageHeight - row;
                var i = column - 1;
                var u = (i + StaticRandom.NextFloat()) / (imageWidth - 1);
                var v = (j + StaticRandom.NextFloat()) / (imageHeight - 1);
                var ray = camera.GetRay(u, v);
                color += RayColor(in ray, background, maxDepth, ref scene);
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

        private static Vec3 RayColor(in Ray ray, in Vec3 background, int depth, ref IHittable target)
        {
            if (depth <= 0)
            {
                return new Vec3(0, 0, 0);
            }

            HitResult hitResult = new HitResult();
            if (target.Hit(ref hitResult, ray, 0.001f, float.PositiveInfinity))
            {
                var emitted = hitResult.Material.Emitted(hitResult.U, hitResult.V, hitResult.P);

                var scatterResult = new ScatterResult();
                if (hitResult.Material.Scatter(ref scatterResult, ray, hitResult))
                {
                    return emitted + scatterResult.Attenuation * RayColor(scatterResult.ScatteredRay, background, depth - 1, ref target);
                }
                else
                {
                    return emitted;
                }
            }
            else
            {
                return background;
            }
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
