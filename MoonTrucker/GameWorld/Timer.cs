using System;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class Timer
    {
        private TimeSpan _timeSpan;

        public Timer(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        public void AddTime(TimeSpan time)
        {
            _timeSpan += time;
        }

        public TimeSpan GetTime()
        {
            var isTimeNegative = _timeSpan < TimeSpan.Zero;
            return isTimeNegative ? TimeSpan.Zero : _timeSpan;
        }

        public bool Update(GameTime gameTime)
        {
            _timeSpan -= gameTime.ElapsedGameTime;

            if (_timeSpan < TimeSpan.Zero)
            {
                return false;
            }

            return true;
        }
    }
}
