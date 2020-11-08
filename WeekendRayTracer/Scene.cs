using System.Collections.Generic;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public class Scene : IHittable
    {
        private readonly IList<IHittable> _objects;

        public Scene()
        {
            _objects = new List<IHittable>();
        }

        public void Add(IHittable obj)
        {
            _objects.Add(obj);
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public HitResult Hit(Ray ray, double tMin, double tMax)
        {
            HitResult finalResult = null;
            var closestSoFar = tMax;

            foreach (var obj in _objects)
            {
                var result = obj.Hit(ray, tMin, closestSoFar);
                if (result != null)
                {
                    closestSoFar = result.T;
                    finalResult = result;
                }
            }

            return finalResult;
        }

    }
}
