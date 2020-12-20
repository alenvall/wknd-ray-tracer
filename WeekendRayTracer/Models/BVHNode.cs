using System;
using System.Collections.Generic;
using WeekendRayTracer.Models.Tracing;

namespace WeekendRayTracer.Models
{
    public readonly struct BVHNode : IHittable
    {
        public IHittable Left { get; }
        public IHittable Right { get; }
        public AABB Box { get; }

        public static BVHNode Root(List<IHittable> objects, float time0, float time1)
        {
            return new BVHNode(objects.ToArray(), 0, objects.Count, time0, time1);
        }

        private BVHNode(in IHittable[] objects, int start, int end, float time0, float time1)
        {
            Comparison<IHittable> comparer = StaticRandom.Next(0, 3) switch
            {
                0 => CompareX,
                1 => CompareY,
                2 => CompareZ,
                _ => throw new NotImplementedException()
            };

            var span = end - start;

            if (span == 1)
            {
                Left = Right = objects[start];
            }
            else if (span == 2)
            {
                if (comparer(objects[start], objects[start + 1]) < 0)
                {
                    Left = objects[start];
                    Right = objects[start + 1];
                }
                else
                {
                    Left = objects[start + 1];
                    Right = objects[start];
                }
            }
            else
            {
                var length = end - start;
                var subArray = new IHittable[length];
                Array.Copy(objects, start, subArray, 0, length);
                Array.Sort(subArray, comparer);
                Array.Copy(subArray, 0, objects, start, length);

                var middle = start + span / 2;
                Left = new BVHNode(in objects, start, middle, time0, time1);
                Right = new BVHNode(in objects, middle, end, time0, time1);
            }

            var boxLeft = new AABB();
            var boxRight = new AABB();

            if (!Left.BoundingBox(ref boxLeft, time0, time1) || !Right.BoundingBox(ref boxRight, time0, time1))
            {
                throw new Exception("No bounding box in node constructor.");
            }

            Box = AABB.SurroundingBox(boxLeft, boxRight);
        }

        public bool Hit(ref HitResult result, in Ray ray, float tMin, float tMax)
        {
            if (!Box.Hit(ref result, in ray, tMin, tMax))
            {
                return false;
            }

            bool hitLeft = Left.Hit(ref result, in ray, tMin, tMax);
            bool hitRight = Right.Hit(ref result, in ray, tMin, hitLeft ? result.T : tMax);

            return hitLeft || hitRight;
        }

        public bool BoundingBox(ref AABB box, float time0, float time1)
        {
            box = Box;
            return true;
        }

        private static int CompareX(IHittable a, IHittable b)
        {
            AABB boxA = new AABB();
            AABB boxB = new AABB();

            if (!a.BoundingBox(ref boxA, 0, 0) || !b.BoundingBox(ref boxB, 0, 0))
            {
                throw new Exception("No bounding box to compare.");
            }

            if (boxA.Minimum.X - boxB.Minimum.X < float.Epsilon)
            {
                return 0;
            }
            else
            {
                return boxA.Minimum.X < boxB.Minimum.X ? -1 : 1;
            }
        }

        private static int CompareY(IHittable a, IHittable b)
        {
            AABB boxA = new AABB();
            AABB boxB = new AABB();

            if (!a.BoundingBox(ref boxA, 0, 0) || !b.BoundingBox(ref boxB, 0, 0))
            {
                throw new Exception("No bounding box to compare.");
            }

            if (boxA.Minimum.Y - boxB.Minimum.Y < float.Epsilon)
            {
                return 0;
            }
            else
            {
                return boxA.Minimum.Y < boxB.Minimum.Y ? -1 : 1;
            }
        }

        private static int CompareZ(IHittable a, IHittable b)
        {
            AABB boxA = new AABB();
            AABB boxB = new AABB();

            if (!a.BoundingBox(ref boxA, 0, 0) || !b.BoundingBox(ref boxB, 0, 0))
            {
                throw new Exception("No bounding box to compare.");
            }

            if (boxA.Minimum.Z - boxB.Minimum.Z < float.Epsilon)
            {
                return 0;
            }
            else
            {
                return boxA.Minimum.Z < boxB.Minimum.Z ? -1 : 1;
            }
        }

    }
}
