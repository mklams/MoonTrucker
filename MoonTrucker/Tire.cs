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
    public class Tire
    {
        private float _maxForwardSpeed = 25;
        private float _maxBackwardSpeed = -5;
        private float _maxDriveForce = 40;

        private readonly float _screenWidth;
        private readonly float _screenHeight;
        private const float IMPULSE_FACTOR = 0.1f;

        private Vector2 _position;
        private float _angle = 0;

        private Sprite _sprite { get; }

        private Body _body { get; }

        public Tire(Sprite vehicleSprite, World world, Vector2 position, float screenWidth, float screenHeight)
        {
            _sprite = vehicleSprite;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _position = position;
            _body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(_sprite.Width), ConvertUnits.ToSimUnits(_sprite.Height), 1f, ConvertUnits.ToSimUnits(_position), _angle, BodyType.Dynamic);
            _body.Restitution = 0.3f;
            _body.Friction = 0.5f;
        }

        public float GetHeight()
        {
            return _sprite.Height;
        }

        public float GetWidth()
        {
            return _sprite.Width;
        }

        private Vector2 getLateralVelocity()
        {
            Vector2 currentRightNormal = _body.GetWorldVector(new Vector2(1, 0));
            return Vector2.Dot(currentRightNormal, _body.LinearVelocity) * currentRightNormal;
        }

        private Vector2 getForwardVelocity()
        {
            Vector2 currentRightNormal = _body.GetWorldVector(new Vector2(0, 1));
            return Vector2.Dot(currentRightNormal, _body.LinearVelocity) * currentRightNormal;
        }

        private void updateFriction()
        {
            Vector2 impulse = _body.Mass * -getLateralVelocity();
            _body.ApplyLinearImpulse(impulse, _body.WorldCenter);
            _body.ApplyAngularImpulse(IMPULSE_FACTOR * _body.Inertia * -_body.AngularVelocity);

            Vector2 currentForwardNormal = getForwardVelocity();
            float currentForwardSpeed = Vector2.Normalize(currentForwardNormal).Length();
            float dragForceMagnitude = -2 * currentForwardSpeed;
            if(!float.IsNaN(dragForceMagnitude))
            {
                _body.ApplyForce(dragForceMagnitude * currentForwardNormal, _body.WorldCenter);
            }
        }

        public void Draw()
        {
            _sprite.Draw(ConvertUnits.ToDisplayUnits(_body.Position), _body.Rotation);
        }

        public void updateDrive(KeyboardState keyboardState)
        {
            float desiredSpeed = 0f;
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                desiredSpeed = _maxForwardSpeed;
            }
            else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                desiredSpeed = _maxBackwardSpeed;
            }
            else
            {
                return;
            }

            Vector2 currentForwardNormal = _body.GetWorldVector(new Vector2(0, 1));
            float currentSpeed = Vector2.Dot(getForwardVelocity(), currentForwardNormal);

            float force = 0;
            if (desiredSpeed > currentSpeed)
            {
                force = _maxDriveForce;
            }
            else if (desiredSpeed < currentSpeed)
            {
                force = -_maxDriveForce;
            }
            else
            {
                return;
            }

            _body.ApplyForce(force * currentForwardNormal, _body.WorldCenter);
        }

        private void updateTurn(KeyboardState keyboardState)
        {
            float desiredTorque = 0f;
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                desiredTorque = 15f;
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                desiredTorque = -15f;
            }

            _body.ApplyTorque(desiredTorque);
        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            updateFriction();
            updateDrive(keyboardState);
            updateTurn(keyboardState);
        }
    }
}
