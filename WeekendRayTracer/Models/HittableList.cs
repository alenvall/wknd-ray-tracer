using System;
using System.Collections.Generic;

namespace WeekendRayTracer.Models
{
    public class HittableList : IHittable
    {
        public IList<IHittable> Objects;

        public HittableList()
        {
            Objects = new List<IHittable>();
        }

        public void Add(IHittable obj)
        {
            Objects.Add(obj);
        }

        public void Clear()
        {
            Objects.Clear();
        }

        public HitRecord Hit(Ray ray, double tMin, double tMax)
        {
            HitRecord record = null;
            var closestSoFar = tMax;

            foreach (var obj in Objects)
            {
                var newRecord = obj.Hit(ray, tMin, closestSoFar);
                if (newRecord != null)
                {
                    closestSoFar = newRecord.T;
                    record = newRecord;
                }
            }

            return record;
        }

    }
}
