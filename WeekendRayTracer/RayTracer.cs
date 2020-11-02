using System;
using System.Collections.Generic;
using System.IO;

namespace WeekendRayTracer
{
    class RayTracer
    {
        private static readonly int IMAGE_WIDTH = 256;
        private static readonly int IMAGE_HEIGHT = 256;

        static void Main(string[] args)
        {
            Log("Creating image...");

            var lines = new List<string>();
            lines.Add($"P3\n{IMAGE_WIDTH} {IMAGE_HEIGHT} \n255\n");

            var remaining = IMAGE_HEIGHT;
            for (int j = IMAGE_HEIGHT - 1; j > 0; --j)
            {
                for (int i = 0; i < IMAGE_WIDTH; ++i)
                {
                    var r = (double)i / (IMAGE_WIDTH - 1);
                    var g = (double)j / (IMAGE_HEIGHT - 1);
                    var b = 0.25;

                    var ir = (int)(255.999 * r);
                    var ig = (int)(255.999 * g);
                    var ib = (int)(255.999 * b);

                    lines.Add($"{ir} {ig} {ib}\n");
                }

                remaining = j;
                Console.Write("\rScanlines remaining: {0}    ", remaining);
            }
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

        private static void Log(string text)
        {
            Console.WriteLine(text);
        }
    }
}
