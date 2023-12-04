﻿using OpenTK.Mathematics;

namespace RockEngine.Figures
{
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public Vector3 GetPoint(float distance)
        {
            return this.Origin + this.Direction * distance;
        }
    }
}
