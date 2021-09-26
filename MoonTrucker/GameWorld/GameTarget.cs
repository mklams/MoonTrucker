using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Handlers;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class GameTarget: IDrawable
    {
        private CircleProp _body;
        public int HitTotal = 0;

        public GameTarget(float radius, Vector2 position, PropFactory bodyFactory, GameMap map)
        {
            // FUTURE: If more things need to run actions on collision use the observer pattern
            OnCollisionHandler onHitAction = (Fixture fixtureA, Fixture fixtureB, Contact contact) => {
                _body.Body.Position = map.GetRandomTargetLocation();
                HitTotal++;
            };

            _body = bodyFactory.CreateCircleSensor(radius, position, onHitAction);
        }

        public void Draw()
        {
            _body.Draw();
        }
    }
}
