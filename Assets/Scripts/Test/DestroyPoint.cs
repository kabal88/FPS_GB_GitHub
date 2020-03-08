using System;
using UnityEngine;

namespace Geekbrains
{
    public sealed class DestroyPoint : MonoBehaviour
    {
        public event Action<GameObject> OnFinishChange = delegate(GameObject o) {  };

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Bot>())
            {
                OnFinishChange.Invoke(gameObject);
            }
        }
    }
}
