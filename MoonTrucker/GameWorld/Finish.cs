using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Handlers;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class Finish: IDrawable
    {
        private CircleProp _body;
        private bool _isActive = false; // Default to not having the finish be active
        private bool _playerHit = false;

        public Finish(float radius, Vector2 position, PropFactory bodyFactory)
        {
            OnCollisionHandler onHitAction = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (_isActive)
                {
                    _playerHit = true;
                }
            };

            OnSeparationHandler onRemoveAction = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                _playerHit = false;
            };

            _body = bodyFactory.CreateCircleSensor(radius, position, onHitAction);
            _body.SetColor(Color.Green);
        }

        public Vector2 GetPosition()
        {
            return _body.Body.Position;
        }

        public bool IsPlayerInFinishZone()
        {
            return _playerHit;
        }

        public void MakeActive()
        {
            _isActive = true;
        }

        public void MakeInactive()
        {
            _isActive = false;
            _playerHit = false;
        }

        public void Draw()
        {
            if (_isActive)
            {
                _body.Draw();
            }
            
        }
    }
}
