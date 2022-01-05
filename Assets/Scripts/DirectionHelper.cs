using System;
using HexFiled;
using UnityEngine;

namespace DefaultNamespace
{
    public static class DirectionHelper
    {
        public static Vector3 DirectionTo(Vector3 a, Vector3 b)
        {
            return (b - a).normalized;
        }
        
        public static HexDirection VectorToDirection(Vector2 dir)
        {
            if (dir.x >= 0 && dir.y <= 1 && dir.x <= 1 && dir.y >= 0.5)
            {
                return HexDirection.NE;
            }

            if (Math.Abs(dir.x - 1f) < 0.2 && dir.y <= 0.5 && dir.y >= -0.5)
            {
                return HexDirection.E;
            }

            if (dir.x <= 1 && dir.y <= -0.5 && dir.x >= 0 && dir.y >= -1)
            {
                return HexDirection.SE;
            }

            if (dir.x <= 0 && dir.y >= -1 && dir.x >= -1 && dir.y <= -0.5)
            {
                return HexDirection.SW;
            }

            if (Math.Abs(dir.x - (-1f)) < 0.2 && dir.y >= -0.5 && dir.y <= 0.5)
            {
                return HexDirection.W;
            }

            if (dir.x >= -1 && dir.y >= 0.5 && dir.x <= 0 && dir.y <= 1)
            {
                return HexDirection.NW;
            }

            return HexDirection.W;
        }

    }
}