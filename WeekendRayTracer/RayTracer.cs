using System;
using System.Collections.Generic;
using System.IO;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models;

namespace WeekendRayTracer
{
    public class RayTracer
    {
        private List<string> lines;
        readonly Random rand = new Random();

        public void Run()
        {
            // Image
            var aspectRatio = 16.0 / 9.0;
            var imageWidth = 400;
            var imageHeight = (int)(imageWidth / aspectRatio);
            int samplesPerPixel = 100;
            int maxDepth = 50;

            // World
            HittableList world = new HittableList();

            var materialGround = new Lambertian(new Vec3(0.8, 0.8, 0.0));
            var materialCenter = new Lambertian(new Vec3(0.1, 0.2, 0.5));
            var materialLeft = new Dielectric(1.5);
            var materialRight = new Metal(new Vec3(0.8, 0.6, 0.2), 0.0);

            world.Add(new Sphere(new Vec3(0.0, -100.5, -1.0), 100.0, materialGround));
            world.Add(new Sphere(new Vec3(0.0, 0.0, -1.0), 0.5, materialCenter));
            world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), 0.5, materialLeft));
            world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), -0.4, materialLeft));
            world.Add(new Sphere(new Vec3(1.0, 0.0, -1.0), 0.5, materialRight));

            var lookFrom = new Vec3(3, 3, 2);
            var lookAt = new Vec3(0, 0, -1);
            var vUp = new Vec3(0, 1, 0);
            var focusDistance = (lookFrom - lookAt).Length();
            var aperture = 2.0;

            Camera cam = new Camera(lookFrom, lookAt, vUp, 20, aspectRatio, aperture, focusDistance);

            Log("Creating image...");

            lines = new List<string>
            {
                $"P3\n{imageWidth} {imageHeight} \n255\n"
            };

            for (int j = imageHeight - 1; j > 0; --j)
            {
                Console.Write("\rScanlines remaining: {0}    ", j);
                for (int i = 0; i < imageWidth; ++i)
                {
                    var pixelColor = new Vec3(0, 0, 0);
                    for (int s = 0; s < samplesPerPixel; ++s)
                    {
                        var u = (i + rand.NextDouble()) / (imageWidth - 1);
                        var v = (j + rand.NextDouble()) / (imageHeight - 1);
                        var ray = cam.GetRay(u, v);
                        pixelColor += RayColor(ray, world, maxDepth);
                    }

                    WriteColor(pixelColor, samplesPerPixel);
                }
            }
            Console.Write("\rScanlines remaining: {0}    ", 0);
            Log("\n\n");

            PrintFile();
            Log("Done!");

            Console.ReadLine();
        }

        private Vec3 RayColor(Ray ray, IHittable world, int depth)
        {
            if (depth <= 0)
            {
                return new Vec3(0, 0, 0);
            }

            var record = world.Hit(ray, 0.001, double.PositiveInfinity);
            if (record != null)
            {
                var scatterResult = record.Materal.Scatter(ray, record);
                if (scatterResult != null)
                {
                    return scatterResult.Attenuation * RayColor(scatterResult.ScatteredRay, world, depth - 1);
                }

                return new Vec3(0, 0, 0);
            }

            var directionUnit = ray.Direction.Unit();
            var t = 0.5 * (directionUnit.Y + 1.0);
            return (1.0 - t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0);
        }

        private void Log(object text)
        {
            Console.WriteLine(text);
        }

        private void WriteColor(Vec3 pixelColor, int samplesPerPixel)
        {
            var r = pixelColor.X;
            var g = pixelColor.Y;
            var b = pixelColor.Z;

            var scale = 1.0 / samplesPerPixel;
            r = Math.Sqrt(scale * r);
            g = Math.Sqrt(scale * g);
            b = Math.Sqrt(scale * b);

            lines.Add($"{(int)(256 * Math.Clamp(r, 0.0, 0.999))} {(int)(256 * Math.Clamp(g, 0.0, 0.999))} {(int)(256 * Math.Clamp(b, 0.0, 0.999))}\n");
        }

        private void PrintFile()
        {
            using StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "image.ppm"));
            foreach (string line in lines)
            {
                outputFile.Write(line);
            }
        }
    }
}
