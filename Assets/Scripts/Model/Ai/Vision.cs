using Sirenix.OdinInspector;
using UnityEngine;

namespace Geekbrains
{
    [System.Serializable]
    public sealed class Vision
    {
        public float ActiveVisionDistance = 10;
        public float ActiveVisionAngle = 35;
        public LayerMask LayerMask;
        public float FeelingDistanceOffset = 15;
        [ShowInInspector]private float _activeFeelingDistance {
            get
            {
                return FeelingDistanceOffset + ActiveVisionDistance;
            }
        }

        public bool VisionM(Transform player, Transform target)
        {
            return Angle(player, target) && !CheckBloked(player, target);
        }

        private bool CheckBloked(Transform player, Transform target)
        {
            if (!Physics.Linecast(player.position, target.position, out var hit, LayerMask)) return true;
            return hit.transform != target;
        }

        private bool Angle(Transform player, Transform target)
        {
            var angle = Vector3.Angle(player.forward, target.position - player.position);
            return angle <= ActiveVisionAngle;
        }

        private bool Distance(Transform player, Transform target)
        {
            var distSqr = CustomVector.DistanceSqr(player.position, target.position);
            return distSqr <= Mathf.Pow(ActiveVisionDistance,2);
        }

        public bool FeelingTarget(Transform player, Transform target)
        {
            return CustomVector.CheckDistanceMatch(player.position, target.position, _activeFeelingDistance);
        }
    }
}
