using System;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Handlers;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class GameTarget : IDrawable, IObservable<GameTarget>
    {
        private CircleProp _body;
        public int HitTotal = 0;

        private List<IObserver<GameTarget>> _observers = new List<IObserver<GameTarget>>();

        public GameTarget(float radius, Vector2 position, PropFactory bodyFactory)
        {
            OnCollisionHandler onHitAction = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                foreach (var obserer in _observers)
                {
                    obserer.OnNext(this);
                }
                HitTotal++;
            };

            _body = bodyFactory.CreateCircleSensor(radius, position, onHitAction);
        }

        public void Draw()
        {
            _body.Draw();
        }

        public void ResetHitTotal()
        {
            HitTotal = 0;
        }

        public void SetPosition(Vector2 position)
        {
            _body.Body.Position = position;
        }

        public Vector2 GetPosition()
        {
            return _body.Body.Position;
        }

        public IDisposable Subscribe(IObserver<GameTarget> observer)
        {
            _observers.Add(observer);

            return new Unsubscriber<GameTarget>(_observers, observer);
        }
    }

    internal class Unsubscriber<GameTarget> : IDisposable
    {
        private List<IObserver<GameTarget>> _observers;
        private IObserver<GameTarget> _observer;

        internal Unsubscriber(List<IObserver<GameTarget>> observers, IObserver<GameTarget> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
