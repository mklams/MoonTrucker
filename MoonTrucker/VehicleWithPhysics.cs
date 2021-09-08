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

        public void Draw()
        {
            _sprite.Draw(ConvertUnits.ToDisplayUnits(_vehicleBody.Position), _vehicleBody.Rotation);
        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                _vehicleBody.ApplyLinearImpulse(new Vector2(0, -5));
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                _vehicleBody.ApplyLinearImpulse(new Vector2(0, 5));
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                _vehicleBody.ApplyLinearImpulse(new Vector2(-5, 0));
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                _vehicleBody.ApplyLinearImpulse(new Vector2(5, 0));
            }
        }
    }
}
