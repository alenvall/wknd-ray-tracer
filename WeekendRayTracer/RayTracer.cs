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

            // World
            HittableList world = new HittableList();
            world.Add(new Sphere(new Vec3(0, 0, -1), 0.5));
            world.Add(new Sphere(new Vec3(0, -100.5, -1), 100));

            Camera cam = new Camera();

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
                        pixelColor += RayColor(ray, world);
                    }

                    WriteColor(pixelColor, samplesPerPixel);
                }
            }
            Console.Write("\rScanlines remaining: {0}    ", 0);
            Console.Write("\n\n");

            PrintFile();
            Log("Done!");
        }

        private Vec3 RayColor(Ray ray, IHittable world)
        {
            var record = world.Hit(ray, 0, double.PositiveInfinity);
            if (record != null)
            {
                return 0.5 * (record.Normal + new Vec3(1, 1, 1));
            }

            var directionUnit = ray.Direction.Unit;
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
            r *= scale;
            g *= scale;
            b *= scale;

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
