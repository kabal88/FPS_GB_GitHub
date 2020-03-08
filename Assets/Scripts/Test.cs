using UnityEngine;

namespace Geekbrains
{
    public class Test : MonoBehaviour
    {
        public Canvas Canvas;
        private void Start()
        {
            FindObjectOfType<FlashLightModel>().Layer = 2;

            gameObject.CompareTag(TagManager.PLAYER);
        }

        private void OnValidate()
        {
            if (TryGetComponent(out Canvas canvas))
            {
                Canvas = canvas;
                Canvas.enabled = false;
            }
        }
    }
}
