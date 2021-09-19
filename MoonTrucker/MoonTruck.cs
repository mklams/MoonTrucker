

using System.Collections.Generic;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MoonTrucker{

    public class MoonTruck : Vehicle{

        private const float HEIGHT_TIRE_MULT = .34f;
        private const float WIDTH_TIRE_MULT = .3f;

        private const float TURN_RATE = 1f;

        private List<Tire> _tires;
        public MoonTruck(float width, 
            float height, 
            Vector2 position, 
            World world, 
            TextureManager manager, 
            SpriteBatch batch, 
            GraphicsDevice graphicsDevice)
        :base(width, height, position, world, manager, batch, graphicsDevice)
        {
            this._initializeTires();
        }

        private void _initializeTires()
        {
            _tires = new List<Tire>();
            //front right wheel
            _tires.Add(new Tire(new Vector2(Height*HEIGHT_TIRE_MULT, Width*WIDTH_TIRE_MULT), canTurn: true, canDrive: true));
            //front left wheel
            _tires.Add(new Tire(new Vector2(Height*HEIGHT_TIRE_MULT, -Width*WIDTH_TIRE_MULT), canTurn: true, canDrive: true));
            //back right wheel
            _tires.Add(new Tire(new Vector2(-Height*HEIGHT_TIRE_MULT, Width*WIDTH_TIRE_MULT), canTurn: false, canDrive: true));
            //back left wheel
            _tires.Add(new Tire(new Vector2(-Height*HEIGHT_TIRE_MULT, -Width*WIDTH_TIRE_MULT), canTurn: false, canDrive: true));
        }

        protected override void handleUpKey()
        {
            if(VectorHelpers.IsMovingForward(_vehicleBody) || VectorHelpers.IsStopped(_vehicleBody))
            {
                _tires.ForEach(t => t.ApplyForwardDriveForce(_vehicleBody));
            }
            else
            {
                _isBraking = true;
                _tires.ForEach(t => t.ApplyBrakeForce(_vehicleBody));
            }
        }
        protected override void handleDownKey()
        {
            if(VectorHelpers.IsMovingForward(_vehicleBody))
            {
                _isBraking = true;
                _tires.ForEach(t => t.ApplyBrakeForce(_vehicleBody));
            }
            else
            {
                _tires.ForEach(t => t.ApplyReverseDriveForce(_vehicleBody));
            }
        }
        protected override void handleLeftKey()
        {
            _tires.ForEach(t => t.Turn(TURN_RATE));
        }
        protected override void handleRightKey()
        {
            _tires.ForEach(t => t.Turn(-TURN_RATE));
        }
        protected override void snapVelocityToZero()
        {
            if(!VectorHelpers.IsStopped(_vehicleBody) && _vehicleBody.LinearVelocity.Length() < .3f){
                _vehicleBody.ResetDynamics();
            }
        }
        protected override void applyFriction()
        {
            _tires.ForEach(t => t.ApplyFrictionForce(_vehicleBody));
        }
        protected override void applyTraction()
        {
            _tires.ForEach(t => t.ApplyTractionForce(_vehicleBody));
        }

    }
}