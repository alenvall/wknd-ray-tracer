using System.Collections.Generic;
using System.Linq;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public readonly struct Scene : IHittable
    {
        private IHittable[] array { get; }

        public Scene(IList<IHittable> objects)
        {
            array = objects.ToArray();
        }

        public bool Hit(ref HitResult finalResult, in Ray ray, float tMin, float tMax)
        {
            var closestSoFar = tMax;
            var hitAnything = false;

            for (int i = 0; i < array.Length; i++)
            {
                var obj = array[i];
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

    }
}
