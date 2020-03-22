using UnityEngine;

namespace Geekbrains
{
    public class Test : MonoBehaviour
    {

        public int Count;
        private TimeRemaining _timer = new TimeRemaining(delegate { }, 1, true);


        private void Start()
        {
            _timer = new TimeRemaining(TestingPrint, 1, true);
            Count = 0;
            TimeRemainingExtensions.Add(_timer);
        }

        //private void OnValidate()
        //{
        //    if (TryGetComponent(out Canvas canvas))
        //    {
        //        Canvas = canvas;
        //        Canvas.enabled = false;
        //    }
        //}

        private void TestingPrint()
        {
            Count++;
            CustomDebug.Log($"Testign timer execute {Count} times!");
        }
    }
}
