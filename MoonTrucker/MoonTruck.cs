

using System.Collections.Generic;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MoonTrucker
{

    public class MoonTruck : Vehicle
    {

        private const float HEIGHT_TIRE_MULT = .34f;
        private const float WIDTH_TIRE_MULT = .3f;

        private const float TURN_RATE = .01f;

        private List<Tire> _tires;
        public MoonTruck(float width,
            float height,
            Vector2 position,
            World world,
            TextureManager manager,
            SpriteBatch batch,
            GraphicsDevice graphicsDevice)
        : base(width, height, position, world, manager, batch, graphicsDevice)
        {
            this._initializeTires();
        }

        private void _initializeTires()
        {
            _tires = new List<Tire>();
            //front right wheel
            _tires.Add(new Tire(new Vector2(Height * HEIGHT_TIRE_MULT, Width * WIDTH_TIRE_MULT), canTurn: true, canDrive: true));
            //front left wheel
            _tires.Add(new Tire(new Vector2(Height * HEIGHT_TIRE_MULT, -Width * WIDTH_TIRE_MULT), canTurn: true, canDrive: true));
            //back right wheel
            _tires.Add(new Tire(new Vector2(-Height * HEIGHT_TIRE_MULT, Width * WIDTH_TIRE_MULT), canTurn: false, canDrive: true));
            //back left wheel
            _tires.Add(new Tire(new Vector2(-Height * HEIGHT_TIRE_MULT, -Width * WIDTH_TIRE_MULT), canTurn: false, canDrive: true));
        }

        protected override void handleUpKey(GameTime gameTime)
        {
            if (VectorHelpers.IsMovingForward(_body) || VectorHelpers.IsStopped(_body))
            {
                _tires.ForEach(t => t.ApplyForwardDriveForce(_body));
            }
            else
            {
                _isBraking = true;
                _tires.ForEach(t => t.ApplyBrakeForce(_body));
            }
        }
        protected override void handleDownKey(GameTime gameTime)
        {
            if (VectorHelpers.IsMovingForward(_body))
            {
                _isBraking = true;
                _tires.ForEach(t => t.ApplyBrakeForce(_body));
            }
            else
            {
                _tires.ForEach(t => t.ApplyReverseDriveForce(_body));
            }
        }
        protected override void handleLeftKey(GameTime gameTime)
        {
            _tires.ForEach(t => t.Turn(-TURN_RATE));
        }
        protected override void handleRightKey(GameTime gameTime)
        {
            _tires.ForEach(t => t.Turn(TURN_RATE));
        }
        protected override void snapVelocityToZero()
        {
            if (!VectorHelpers.IsStopped(_body) && _body.LinearVelocity.Length() < .3f)
            {
                _body.ResetDynamics();
            }
        }
        protected override void applyFriction()
        {
            _tires.ForEach(t => t.ApplyFrictionForce(_body));
            _tires.ForEach(t => t.ApplyTractionForce(_body));
        }

        protected override void handleSpaceBar(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }

    }
}