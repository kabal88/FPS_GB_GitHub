using UnityEngine;

namespace Geekbrains
{
    [System.Serializable]
    public sealed class Sence
    {
        public float ActiveDistance = 15;

        public bool FeelingTarget(Transform player, Transform target)
        {

            return CustomVector.CheckDistanceMatch(player.position, target.position, ActiveDistance);
        }

    }
}
