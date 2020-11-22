using System.Collections.Generic;
using WeekendRayTracer.Models;
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

    }
}
