using System;
using System.Collections.Generic;
using System.IO;

namespace WeekendRayTracer
{
    class RayTracer
    {
        private static readonly int IMAGE_WIDTH = 256;
        private static readonly int IMAGE_HEIGHT = 256;
        private static List<string> lines;

        static void Main(string[] args)
        {
            Log("Creating image...");

            lines = new List<string>();
            lines.Add($"P3\n{IMAGE_WIDTH} {IMAGE_HEIGHT} \n255\n");

            var remaining = IMAGE_HEIGHT;
            for (int j = IMAGE_HEIGHT - 1; j > 0; --j)
            {
                for (int i = 0; i < IMAGE_WIDTH; ++i)
                {
                    var color = new Vec3((double)i / (IMAGE_WIDTH - 1), (double)j / (IMAGE_HEIGHT - 1), 0.25);
                    WriteColor(color);
                }

                remaining = j;
                Console.Write("\rScanlines remaining: {0}    ", remaining);
            }
            Console.Write("\rScanlines remaining: {0}    ", 0);
            Console.Write("\n\n");

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "image.ppm")))
            {
                foreach (string line in lines)
                {
                    outputFile.Write(line);
                }
            }

            Log("Done!");
        }

        private static void Log(object text)
        {
            Console.WriteLine(text);
        }

        private static void WriteColor(Vec3 color)
        {
            lines.Add($"{(int)(255.999 * color.X)} {(int)(255.999 * color.Y)} {(int)(255.999 * color.Z)}\n");
        }
    }
}
