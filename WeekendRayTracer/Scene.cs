using System;
using System.Collections.Generic;
using WeekendRayTracer.Extensions;
using WeekendRayTracer.Models;
using WeekendRayTracer.Models.Materials;
using WeekendRayTracer.Models.Textures;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public struct Scene : IHittable
    {
        public List<IHittable> Objects { get; set; }

        public void Add(IHittable hittable)
        {
            if (Objects == null)
            {
                Objects = new List<IHittable>();
            }

            Objects.Add(hittable);
        }

        public bool Hit(ref HitResult finalResult, in Ray ray, float tMin, float tMax)
        {
            var closestSoFar = tMax;
            var hitAnything = false;

            foreach (var obj in Objects)
            {
                var tempResult = new HitResult();
                if (obj.Hit(ref tempResult, ray, tMin, closestSoFar))
                {
                    hitAnything = true;
                    closestSoFar = tempResult.T;
                    finalResult = tempResult;
                }
            }

            return hitAnything;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            if (Objects.Count == 0)
            {
                return false;
            }

            AABB tempBox = new AABB();
            var firstBox = true;

            foreach (var obj in Objects)
            {
                if (!obj.BoundingBox(ref tempBox, time0, time1))
                {
                    return false;
                }

                box = firstBox ? tempBox : AABB.SurroundingBox(box, tempBox);
                firstBox = false;
            }

            return true;
        }

        public static Scene RandomSphereScene(int complexity)
        {
            var scene = new Scene();
            var rand = new Random();

            var checker = new CheckerTexture(new Vec3(0.2f, 0.3f, 0.1f), new Vec3(0.9f, 0.9f, 0.9f));
            scene.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(checker)));

            var objects = new List<IHittable>();

            for (int a = -complexity; a < complexity; a++)
            {
                for (int b = -complexity; b < complexity; b++)
                {
                    var chooseMaterial = rand.NextDouble();
                    var center = new Vec3(a + 0.9f + rand.NextFloat(), 0.2f, b + 0.9f * rand.NextFloat());

                    if ((center - new Vec3(4, 0.2f, 0)).Length() > 0.9f)
                    {
                        IMaterial sphereMaterial;

                        if (chooseMaterial < 0.8f)
                        {
                            // Diffuse
                            var albedo = Vec3.Random() * Vec3.Random();
                            sphereMaterial = new Lambertian(albedo);
                            var center2 = center + new Vec3(0, rand.NextFloat(0, 0.5f), 0);
                            objects.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                        else if (chooseMaterial < 0.95)
                        {
                            // Metal
                            var albedo = Vec3.Random(0.5f, 1);
                            var fuzz = rand.NextFloat(0, 0.5f);
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

            scene.Add(BVHNode.Root(objects, 0, 1.0f));

            return scene;
        }

        public static Scene TwoCheckerSpheres()
        {
            var objects = new List<IHittable>();

            var checker = new CheckerTexture(new Vec3(0.2f, 0.3f, 0.1f), new Vec3(0.9f, 0.9f, 0.9f));
            objects.Add(new Sphere(new Vec3(0, -10, 0), 10, new Lambertian(checker)));
            objects.Add(new Sphere(new Vec3(0, 10, 0), 10, new Lambertian(checker)));

            var scene = new Scene();
            scene.Add(BVHNode.Root(objects, 0, 1.0f));

            return scene;
        }

    }
}
