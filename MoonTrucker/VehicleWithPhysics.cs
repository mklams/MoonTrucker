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
        private readonly float _screenWidth;
        private readonly float _screenHeight;
        private const float IMPULSE_FACTOR = .2f;
        private const float TRACT_FACT = .2f;
        private const float TURN_FACTOR = 1f;

        private Vector2 _position;
        private float _angle = 0;

        private Sprite _sprite { get; }

        private Body _vehicleBody { get; }

        public VehicleWithPhysics(Sprite vehicleSprite, World world, Vector2 position, float screenWidth, float screenHeight)
        {
            _sprite = vehicleSprite;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _position = position;
            _vehicleBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(_sprite.Width), ConvertUnits.ToSimUnits(_sprite.Height), 1f, ConvertUnits.ToSimUnits(_position), _angle, BodyType.Dynamic);
            _vehicleBody.Restitution = 0.3f;
            _vehicleBody.Friction = 0.5f;
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
            _sprite.Draw(ConvertUnits.ToDisplayUnits(_vehicleBody.Position), _vehicleBody.Rotation);
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
            this.applyFriction();
            //this.applyTraction();

            //Vector2 velocity = _vehicleBody.GetLinearVelocityFromLocalPoint()
        }

        private void snapVelocityToZero()
        {
            if(_vehicleBody.LinearVelocity.Length() < .1f){
                _vehicleBody.LinearVelocity = Vector2.Zero;
            }
        }

        private void applyFriction(){
            _vehicleBody.LinearVelocity *= .98f;
            _vehicleBody.AngularVelocity *= .98f;
        }

        private void applyTraction()
        {
            if(_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var velocity = this.copyVector(_vehicleBody.LinearVelocity);
                velocity.Normalize();
                var directionVec = this.getUnitDirectionVector();
                var degrees = MathHelper.ToDegrees(MathF.Acos(Vector2.Dot(velocity, directionVec)));
                _vehicleBody.LinearVelocity = this.rotate(velocity, TRACT_FACT*degrees);
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
