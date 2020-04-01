using System.Collections.Generic;


namespace Geekbrains
{
    public sealed class TimeRemainingController : IExecute
    {
        #region Fields

        private readonly List<TimeRemaining> _timeRemainings;

        #endregion


        #region ClassLifeCycles

        public TimeRemainingController()
        {
            _timeRemainings = TimeRemainingExtensions.TimeRemainings;
        }

        #endregion


        #region IExecute

        public void Execute()
        {
            var time = ServiceTimer.DeltaTime;
            for (int i = 0; i < _timeRemainings.Count; i++)

            {
                var obj = _timeRemainings[i];
                obj.CurrentTime -= time;

                if (obj.CurrentTime <= 0.0f)
                {
                    obj?.Method?.Invoke();
                    CustomDebug.Log($"Invoke method {obj.Method}");
                    if (!obj.IsRepeating)
                    {
                        obj.Remove();
                    }
                    else
                    {
                        obj.CurrentTime = obj.Time;
                    }
                }
            }
        }

        #endregion
    }
}

