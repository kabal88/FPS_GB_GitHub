using UnityEngine;

namespace Geekbrains
{
    public sealed class ArmBot : BaseObjectScene
    {
        [SerializeField] private float _rotationSpeed = 100;

        public void AimingToTarget(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion lookAt = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed);
            transform.rotation = lookAt;
        }
    }
}