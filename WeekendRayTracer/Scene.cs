using System.Collections.Generic;
using System.Linq;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer
{
    public class Scene : IHittable
    {
        private IList<IHittable> _objects { get; }
        private IHittable[] _asArray;

        public Scene()
        {
            _objects = new List<IHittable>();
        }

        public void Add(IHittable obj)
        {
            _objects.Add(obj);
            _asArray = _objects.ToArray();
        }

        public bool Hit(ref HitResult finalResult, in Ray ray, double tMin, double tMax)
        {
            var closestSoFar = tMax;
            var hitAnything = false;

            for (int i = 0; i < _asArray.Length; i++)
            {
                var obj = _asArray[i];
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
