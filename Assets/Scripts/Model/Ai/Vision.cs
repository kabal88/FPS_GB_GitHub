using UnityEngine;

namespace Geekbrains
{
    [System.Serializable]
    public sealed class Vision
    {
        public float ActiveDis = 10;
        public float ActiveAng = 35;
        public LayerMask LayerMask;

        public bool VisionM(Transform player, Transform target)
        {
            //CustomDebug.Log($"Distance - {Distance(player, target)}, Angle - {Angle(player, target)}, CheckBloked - {CheckBloked(player, target)}");
            return Distance(player, target) && Angle(player, target) && !CheckBloked(player, target);
        }

        private bool CheckBloked(Transform player, Transform target)
        {
            if (!Physics.Linecast(player.position, target.position, out var hit, LayerMask)) return true;
            return hit.transform != target;
        }

        private bool Angle(Transform player, Transform target)
        {
            var angle = Vector3.Angle(player.forward, target.position - player.position);
            return angle <= ActiveAng;
        }

        private bool Distance(Transform player, Transform target)
        {
            var dist = Mathf.Pow(player.position.x - target.position.x, 2) + Mathf.Pow(player.position.y - target.position.y, 2);
            return dist <= Mathf.Pow(ActiveDis,2);
        }
    }
}
