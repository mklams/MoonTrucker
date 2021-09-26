using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Handlers;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class GameTarget: IDrawable
    {
        public readonly CircleProp Body;

        public GameTarget(float radius, Vector2 position, PropFactory bodyFactory, MoonTruckerGame game)
        {
            // FUTURE: If more things need to run actions on collision use the observer pattern
            OnCollisionHandler onHitAction = (Fixture fixtureA, Fixture fixtureB, Contact contact) => {
                game.MoveTarget();
            };

            Body = bodyFactory.CreateCircleSensor(radius, position, onHitAction);
        }

        public void Draw()
        {
            Body.Draw();
        }
    }
}
