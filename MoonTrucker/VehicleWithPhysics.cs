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
                _vehicleBody.Rotation += MathHelper.ToRadians(-2f);
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                _vehicleBody.Rotation += MathHelper.ToRadians(2f);
            }

            //Vector2 velocity = _vehicleBody.GetLinearVelocityFromLocalPoint()
        }

        private void handleUpKey()
        {
            Vector2 impulse;
            if(_vehicleBody.LinearVelocity.Length() == 0f)//stopped
            {
                impulse = new Vector2(MathF.Cos(_vehicleBody.Rotation), MathF.Sin(_vehicleBody.Rotation));
            }
            else if(this.isMovingForward()) //accelerate
            {
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
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
            if(_vehicleBody.LinearVelocity.Length() == 0f)//stopped
            {
                impulse = new Vector2(MathF.Cos(_vehicleBody.Rotation), MathF.Sin(_vehicleBody.Rotation));
                impulse *= -1;
            }
            else if(this.isMovingForward()) //decelerate
            {
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            else//acelerate
            {
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
            }
            _vehicleBody.ApplyLinearImpulse(impulse*IMPULSE_FACTOR);
        }

        private Vector2 copyVector(Vector2 vector)
        {
            float vx,vy;
            _vehicleBody.LinearVelocity.Deconstruct(out vx, out vy);
            return new Vector2(vx,vy);
        }
    }
}
