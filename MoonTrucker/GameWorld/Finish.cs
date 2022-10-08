using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Handlers;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using MoonTrucker.Core;

namespace MoonTrucker.GameWorld
{
    public class Finish : IDrawable
    {
        private CircleProp _body;
        private bool _isActive = false; // Default to not having the finish be active
        private bool _playerHit = false;
        private TextureManager _textureManager;

        public Finish(float radius, Vector2 position, PropFactory bodyFactory, TextureManager texMan)
        {
            _textureManager = texMan;
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

            _body = bodyFactory.CreateCircleSensor(radius, position, _textureManager.GetTexture("CheckeredFlag"), onHitAction);
            _body.SetColor(Color.White);
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
