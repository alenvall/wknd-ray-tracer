using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models;
using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public class RayTracer
    {
        private HittableList _scene;
        private Camera _camera;

        private static readonly Random _rand = new Random();

        public void Run()
        {
            // Image
            var aspectRatio = 16.0 / 9.0;
            var imageWidth = 200;
            var imageHeight = (int)(imageWidth / aspectRatio);
            var samplesPerPixel = 50;
            var maxDepth = 50;
            var complexity = 3;
            var renderName = $"{imageWidth}x{imageHeight}_{complexity}_{samplesPerPixel}_{maxDepth}";

            _camera = SetupStandardCamera(aspectRatio);
            _scene = GenerateRandomScene(complexity);

            Log("Rendering scene...");
            var pixels = RenderPixels(imageWidth, imageHeight, samplesPerPixel, maxDepth);

            Log("\n\n");

            PrintFile(imageWidth, imageHeight, pixels, renderName);
            Log("Done! Press any key to exit...");

            Console.ReadLine();
        }

        private List<Vec3> RenderPixels(int imageWidth, int imageHeight, int samples, int maxDepth)
        {
            var pixels = new List<Vec3>();

            for (int j = imageHeight - 1; j > 0; --j)
            {
                Console.Write("\rScanlines remaining: {0}    ", j);
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
            }
            Console.Write("\rScanlines remaining: {0}    ", 0);

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

        private HittableList GenerateRandomScene(int complexity)
        {
            var scene = new HittableList();

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

        private Camera SetupStandardCamera(double aspectRatio)
        {
            var lookFrom = new Vec3(13, 2, 3);
            var lookAt = new Vec3(0, 0, 0);
            var vUp = new Vec3(0, 1, 0);
            var focusDistance = 10.0;
            var aperture = 0.1;

            return new Camera(lookFrom, lookAt, vUp, 20, aspectRatio, aperture, focusDistance);
        }

        private HittableList GenerateSimpleScene()
        {
            HittableList scene = new HittableList();

            var materialGround = new Lambertian(new Vec3(0.8, 0.8, 0.0));
            var materialCenter = new Lambertian(new Vec3(0.1, 0.2, 0.5));
            var materialLeft = new Dielectric(1.5);
            var materialRight = new Metal(new Vec3(0.8, 0.6, 0.2), 0.0);

            scene.Add(new Sphere(new Vec3(0.0, -100.5, -1.0), 100.0, materialGround));
            scene.Add(new Sphere(new Vec3(0.0, 0.0, -1.0), 0.5, materialCenter));
            scene.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), 0.5, materialLeft));
            scene.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), -0.4, materialLeft));
            scene.Add(new Sphere(new Vec3(1.0, 0.0, -1.0), 0.5, materialRight));

            return scene;
        }

        private Camera SetupSimpleCamera(double aspectRatio)
        {
            var lookFrom = new Vec3(3, 3, 2);
            var lookAt = new Vec3(0, 0, -1);
            var vUp = new Vec3(0, 1, 0);
            var focusDistance = (lookFrom - lookAt).Length();
            var aperture = 0.1;

            return new Camera(lookFrom, lookAt, vUp, 20, aspectRatio, aperture, focusDistance);
        }

        private void Log(object text)
        {
            Console.WriteLine(text);
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
