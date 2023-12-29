using OpenTK.Mathematics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockEngine.ECS.Figures
{
    public struct Plane
    {
        public Vector3 Normal;
        public Vector3 Point;

        public Plane(Vector3 normal, Vector3 point)
        {
            Normal = normal;
            Point = point;
        }

        public bool Raycast(Ray ray, out float enter)
        {
            enter = 0;
            float denominator = Vector3.Dot(ray.Direction, Normal);

            // If the ray is parallel to the plane (denominator near zero), there's no intersection
            if(Math.Abs(denominator) < 0.0001f)
            {
                return false;
            }

            float numerator = Vector3.Dot(Point - ray.Origin, Normal);
            enter = numerator / denominator;

            // If enter is negative, the plane is behind the ray's origin
            if(enter < 0.0f)
            {
                return false;
            }

            return true;
        }
    }
}
