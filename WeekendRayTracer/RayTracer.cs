using System;
using System.Collections.Generic;
using System.IO;
using WeekendRayTracer.Models;

namespace WeekendRayTracer
{
    public class RayTracer
    {
        private List<string> lines;

        public void Run()
        {
            // Image
            var aspectRatio = 16.0 / 9.0;
            var imageWidth = 400;
            var imageHeight = (int)(imageWidth / aspectRatio);

            // World
            HittableList world = new HittableList();
            world.Add(new Sphere(new Vec3(0, 0, -1), 0.5));
            world.Add(new Sphere(new Vec3(0, -100.5, -1), 100));

            // Camera
            var viewportHeight = 2.0;
            var viewportWidth = aspectRatio * viewportHeight;
            var focalLength = 1.0;

            var origin = new Vec3(0, 0, 0);
            var horizontal = new Vec3(viewportWidth, 0, 0);
            var vertical = new Vec3(0, viewportHeight, 0);
            var lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - new Vec3(0, 0, focalLength);

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
                    var u = (double)i / (imageWidth - 1);
                    var v = (double)j / (imageHeight - 1);
                    var ray = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
                    var color = RayColor(ray, world);
                    WriteColor(color);
                }
            }
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

        private void WriteColor(Vec3 color)
        {
            lines.Add($"{(int)(255.999 * color.X)} {(int)(255.999 * color.Y)} {(int)(255.999 * color.Z)}\n");
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
