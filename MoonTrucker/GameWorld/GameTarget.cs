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
        public readonly bool IsMovingTarget;
        public int HitTotal = 0;
        private bool _isHidden = false;

        private List<IObserver<GameTarget>> _observers = new List<IObserver<GameTarget>>();

        public GameTarget(float radius, Vector2 position, PropFactory bodyFactory, bool isMoving = false)
        {
            IsMovingTarget = isMoving;

            OnCollisionHandler onHitAction = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(this);
                }
                HitTotal++;
            };
            
            _body = bodyFactory.CreateCircleSensor(radius, position, onHitAction);
        }

        public void Draw()
        {
            if(!_isHidden)
            {
                _body.Draw();
            }
        }

        public void Hide()
        {
            _isHidden = true;
            _body.Body.Enabled = false;
        }

        public void Show()
        {
            _isHidden = false;
            _body.Body.Enabled = true;
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
