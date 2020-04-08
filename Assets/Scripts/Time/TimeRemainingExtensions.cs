using System.Collections.Generic;


namespace Geekbrains
{
    public static partial class TimeRemainingExtensions
    {
        #region Fields

        private static readonly List<TimeRemaining> _timeRemainings = new List<TimeRemaining>(10);

        #endregion


        #region Properties

        public static List<TimeRemaining> TimeRemainings => _timeRemainings;

        #endregion


        #region Methods

        public static void Add(this TimeRemaining value)
        {
            if (_timeRemainings.Contains(value))
            {
                return; 
            }
            _timeRemainings.Add(value);
            CustomDebug.Log($"Timer Add {value}");
        }

        public static void Remove(this TimeRemaining value)
        {
            if (!_timeRemainings.Contains(value))
            {
                return;
            }
            _timeRemainings.Remove(value);

        }

        #endregion

    }
}