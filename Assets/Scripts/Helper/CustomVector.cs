using System;
using UnityEngine;

namespace Geekbrains
{
    public static class CustomVector
    {
        public static float DistanceSqr(Vector3 a, Vector3 b)
        {
            var dist = DistanceSqr(a, b);
            dist += Mathf.Pow(b.z - a.z, 2);
            return dist;
        }


        public static float DistanceSqr(Vector2 a, Vector2 b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            var dist = Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2);
            return dist;
        }

    }
}

