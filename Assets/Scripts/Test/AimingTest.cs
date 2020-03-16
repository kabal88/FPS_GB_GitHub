using UnityEngine;

public class AimingTest : MonoBehaviour
{

    public float RotationSpeed = 20;
    public Transform Target;



    void Update()
    {
        AimingToTarget(Target.position);
    }

    private void AimingToTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion lookAt = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        transform.rotation = lookAt;
        //float angel = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //Quaternion rotation = Quaternion.AngleAxis(angel, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
    }
}
