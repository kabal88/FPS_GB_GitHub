using UnityEngine;

namespace Geekbrains
{
    public static class ServiceTimer
    {
        private static float _deltaTime;
        public static float DeltaTime { get => Time.deltaTime; private set => _deltaTime = value; }
    }
}