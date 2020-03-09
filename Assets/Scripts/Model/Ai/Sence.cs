using UnityEngine;

namespace Geekbrains
{
    [System.Serializable]
    public sealed class Sence
    {
        public float ActiveDistance = 15;

        public bool FeelingTarget(Transform player, Transform target)
        {
            var dist = Vector3.Distance(player.position, target.position); //todo оптимизация
            return dist <= ActiveDistance;
        }

    }
}
