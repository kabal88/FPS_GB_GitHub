using UnityEngine;

public class SpawnTester : MonoBehaviour
{
    #region PrivateData

    #endregion


    #region Fields

    public float Max;
    public float Min;

    #endregion


    #region Properties

    #endregion


    #region UnityMethods

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Min);
        Gizmos.DrawWireSphere(transform.position, Max);
    }

    #endregion


    #region Methods

    #endregion
}
