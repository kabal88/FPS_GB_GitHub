namespace Geekbrains
{
    public static class ServiceTimer
    {
        private static float _deltaTime;
        public static float DeltaTime { get => _deltaTime; set => _deltaTime = value; }
    }
}