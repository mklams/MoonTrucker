using System;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class Timer : IObserver<GameTarget>
    {
        private TimeSpan _timeSpan;
        private IDisposable _cancellation;

        public Timer(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        public void SetTime(TimeSpan time)
        {
            _timeSpan = (time > TimeSpan.Zero) ? time : TimeSpan.Zero;
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

        public void Update(GameTime gameTime)
        {
            _timeSpan -= gameTime.ElapsedGameTime;
        }

        public bool IsTimerUp()
        {
            return _timeSpan < TimeSpan.Zero;
        }

        #region IObserver<GameTarget> Implementation
        public virtual void Subscribe(GameTarget target)
        {
            _cancellation = target.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            _cancellation.Dispose();
        }

        public void OnCompleted()
        {
            // TODO: Remove Target
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(GameTarget target)
        {
            AddTime(TimeSpan.FromSeconds(8));
        }
        #endregion
    }
}
