using System;
using UnityEngine;

namespace Geekbrains
{
    public class DetectorBot : BaseObjectScene
    {
        #region PrivateData

        #endregion


        #region Fields

        public event Action<ITargeted> OnTargetDetected = delegate { };
        public event Action<ITargeted> OnTargetLost = delegate { };

        #endregion


        #region Properties

        #endregion


        #region UnityMethods

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<ITargeted>(out var target))
            {
                OnTargetDetected.Invoke(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<ITargeted>(out var target))
            {
                OnTargetLost.Invoke(target);
            }
        }

        #endregion


        #region Methods

        #endregion
    }
}