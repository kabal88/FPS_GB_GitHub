using UnityEngine;

namespace Geekbrains
{
    [System.Serializable]
    public sealed class Sence
    {
        public float ActiveDistance = 15;

        public bool FeelingTarget(Transform player, Transform target)
        {
            var distSqr = CustomVector.DistanceSqr(player.position, target.position);
            return distSqr <= Mathf.Pow(ActiveDistance,2);
        }

    }
}
