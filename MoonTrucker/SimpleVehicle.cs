using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonTrucker
{
    public class SimpleVehicle : Vehicle
    {
        public SimpleVehicle(float width, float height, Vector2 position, World world, TextureManager manager, SpriteBatch batch, GraphicsDevice graphicsDevice)
            : base(width, height, position, world, manager, batch, graphicsDevice)
        {

        }

        protected override void applyFriction()
        {
            throw new System.NotImplementedException();
        }

        protected override void applyTraction()
        {
            throw new System.NotImplementedException();
        }

        protected override void handleDownKey()
        {
            throw new System.NotImplementedException();
        }

        protected override void handleLeftKey()
        {
            throw new System.NotImplementedException();
        }

        protected override void handleRightKey()
        {
            throw new System.NotImplementedException();
        }

        protected override void handleUpKey()
        {
            throw new System.NotImplementedException();
        }

        protected override void snapVelocityToZero()
        {
            throw new System.NotImplementedException();
        }
    }


}