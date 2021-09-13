using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;

namespace MoonTrucker
{
    public class VehicleWithPhysics
    {
        private const float IMPULSE_FACTOR = .2f;
        private const float TRACT_FACT = .03f;
        private const float TURN_FACTOR = 1f;

        private float _angle = 0;

        private Texture2D _sprite { get; }

        private Body _vehicleBody { get; }
        private SpriteBatch _batch;

        public VehicleWithPhysics(World world, TextureManager manager, SpriteBatch batch)
        {
            _vehicleBody = BodyFactory.CreateRectangle(world, 1f, 0.5f, 1f, new Vector2(7f, 7f), _angle, BodyType.Dynamic);
            _vehicleBody.Restitution = 0.3f;
            _vehicleBody.Friction = 0.5f;

            _sprite = manager.TextureFromShape(_vehicleBody.FixtureList[0].Shape, Color.Black, Color.Red);
            _batch = batch;
        }

        public float GetHeight()
        {
            return _sprite.Height;
        }

        public float GetWidth()
        {
            return _sprite.Width;
        }

        public bool isMovingForward()
        {
            var forwardVector = new Vector2(MathF.Cos(_vehicleBody.Rotation), MathF.Sin(_vehicleBody.Rotation));
            var velVector = this.copyVector(_vehicleBody.LinearVelocity);
            velVector.Normalize();
            return Vector2.Dot( forwardVector, velVector) > 0;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_vehicleBody.Position), null, Color.White, _vehicleBody.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                this.handleUpKey();
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                this.handleDownKey();
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                this.handleLeftKey();
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                this.handleRightKey();
            }
            this.snapVelocityToZero();
            this.applyRotationalFriction();
            this.applyTraction();

        }

        private void snapVelocityToZero()
        {
            if(_vehicleBody.LinearVelocity.Length() < .1f){
                _vehicleBody.LinearVelocity = Vector2.Zero;
            }
        }

        private void applyRotationalFriction(){
            _vehicleBody.AngularVelocity *= .98f;
            //_vehicleBody.LinearVelocity *= .98f;
        }

        private void applyTraction()
        {
            if(_vehicleBody.LinearVelocity.Length() != 0f)
            {
                _vehicleBody.ApplyLinearImpulse(-_vehicleBody.LinearVelocity * TRACT_FACT);
            }
        }

        private void handleLeftKey()
        {
            if(_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var posNeg = this.isMovingForward()? -1 : 1;
                _vehicleBody.ApplyTorque(posNeg * TURN_FACTOR);
            }
        }

        private void handleRightKey()
        {
            if(_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var posNeg = this.isMovingForward()? 1 : -1;
                _vehicleBody.ApplyTorque(posNeg * TURN_FACTOR);
            }
        }

        private Vector2 rotate(Vector2 vector, float degrees) 
        {
            float Vx, Vy;
            Vx = vector.Length()*MathF.Cos(_angle + MathHelper.ToRadians(degrees));
            Vy = vector.Length()*MathF.Sin(_angle + MathHelper.ToRadians(degrees));
            return new Vector2(Vx, Vy);
        }

        private void handleUpKey()
        {
            Vector2 impulse;
            if(_vehicleBody.LinearVelocity.Length() == 0f || (this.isMovingForward()))//stopped or accelerating
            {
                impulse = this.getUnitDirectionVector();
            }
            else//decelerate
            {
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            _vehicleBody.ApplyLinearImpulse(impulse*IMPULSE_FACTOR);
        }

        private void handleDownKey()
        {
            Vector2 impulse;
            if(_vehicleBody.LinearVelocity.Length() == 0f || !this.isMovingForward())//stopped
            {
                impulse = this.getUnitDirectionVector();
                impulse *= -1;
            }
            else //decelerate
            {
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            _vehicleBody.ApplyLinearImpulse(impulse*IMPULSE_FACTOR);
        }

        private Vector2 copyVector(Vector2 vector)
        {
            float vx,vy;
            _vehicleBody.LinearVelocity.Deconstruct(out vx, out vy);
            return new Vector2(vx,vy);
        }

        private Vector2 getUnitDirectionVector()
        {
            return new Vector2(MathF.Cos(_vehicleBody.Rotation), MathF.Sin(_vehicleBody.Rotation));
        }
    }
}
