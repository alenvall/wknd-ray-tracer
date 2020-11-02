using System;

namespace WeekendRayTracer
{
    class RayTracer
    {
        private static readonly int IMAGE_WIDTH = 256;
        private static readonly int IMAGE_HEIGHT = 256;

        static void Main(string[] args)
        {
            Log("Creating image...");

            Console.Write($"P3\n{IMAGE_WIDTH} {IMAGE_HEIGHT} \n255\n");

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

                    Console.Write($"{ir} {ig} {ib}\n");
                }

                remaining = j;
                Console.Error.Write("\rScanlines remaining: {0}", remaining);
            }

            Console.Error.Write("\rScanlines remaining: {0}\n\n", remaining);

            Log("Done!");
        }

        private static void Log(string text)
        {
            Console.Error.WriteLine(text);
        }
    }
}
