using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class Vehicle
    {
        private readonly float _screenWidth;
        private readonly float _screenHeight;

        public Vector2 _origin;
        private Vector2 _position;
        private Vector2 _direction => new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
        private float _speed = 5f;
        private float _angle = 0;

        private const float _rotationVelocity = 3f;
        private const float _linearVelocity = 4f;

        private Sprite _sprite { get; set; }

        public Vehicle(Sprite vehicleSprite, Vector2 position, float screenWidth, float screenHeight)
        {
            _sprite = vehicleSprite;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _position = position;
        }

        public void Draw()
        {
            _sprite.Draw(_position, _angle);
        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            // TODO: Maybe move it's own class that Vehicle implemnts(e.g. IDrivable)
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                moveForward();
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                moveBack();
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                turnLeft();
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                turnRight();
            }

            _position = Vector2.Clamp(_position, new Vector2(0, 0), new Vector2(_screenWidth, _screenHeight));
        }

        private void moveForward()
        {
            _position += _direction * _linearVelocity;
        }
        private void moveBack()
        {
            _position -= _direction * _linearVelocity;
        }

        private void turnLeft()
        {
            _angle -= MathHelper.ToRadians(_rotationVelocity);
        }

        private void turnRight()
        {
            _angle += MathHelper.ToRadians(_rotationVelocity);
        }
    }
}
