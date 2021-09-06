using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class VehicleWithMomentumPolar
    {
        private readonly float _screenWidth;
        private readonly float _screenHeight;
        private Vector2 _velocity => _speed * _direction;
        private float _speed;
        private Vector2 _direction => new Vector2(MathF.Cos(MathHelper.ToRadians(_angle)), MathF.Sin(MathHelper.ToRadians(_angle)));
        /*
         * Angle in degrees
         */
        private float _angle;
        private Vector2 _position;
        private Sprite _sprite { get; set; }
        private bool _forward;
        private bool _stopped;
        private const float MAX_SPEED = 3f;
        private const float MIN_SPEED = .5f;
        private const float DEGREES_TO_ROTATE = 2f;

        public VehicleWithMomentumPolar(Sprite sprite, Vector2 pos, float screenWidth, float screenHeight)
        {
            _position = pos;
            _speed = 0f;
            _forward = true;
            _stopped = true;
            _sprite = sprite;
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
        }

        public void Draw()
        {
            _sprite.Draw(_position, _angle);
        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                handleUpKey();
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                handleDownKey();
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                handleLeftKey();
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                handleRightKey();
            }
            _position += _velocity;
            _speed *= .99f; //friction
            var clampedPos = Vector2.Clamp(_position, new Vector2(0, 0), new Vector2(_screenWidth, _screenHeight));
            if(clampedPos != _position){
                _stopped = true;
                _speed = 0f;
                _position = clampedPos;
            } 
        }

        private void handleUpKey()
        {
            if(_stopped)
            {
                _stopped = false;
                _forward = true;
                _speed = 1f;
            }
            else if(_forward)
            {
                accelerate();
            }
            else
            {
                decelerate();
            }
        }

        private void handleDownKey()
        {
            if(_stopped)
            {
                _stopped = false;
                _forward = false;
                _speed = -1f;
            }
            else if(!_forward){
                accelerate();
            }
            else{
                decelerate();
            }
        }

        private void handleLeftKey()
        {
            if(_stopped)
            {
                return;
            }
            var degreesToRotate = (_forward)? -DEGREES_TO_ROTATE : DEGREES_TO_ROTATE;
            _angle += degreesToRotate;
        }

        private void handleRightKey()
        {
            if(_stopped)
            {
                return;
            }
            var degreesToRotate = (_forward)? DEGREES_TO_ROTATE : -DEGREES_TO_ROTATE;
            _angle += degreesToRotate;
        }

        private void accelerate()
        {
            _speed += 0.5f;
            if(_speed > MAX_SPEED){
                _speed = MAX_SPEED;
            }
        }

        private void decelerate()
        {
            _speed*=.8f;
            if(_speed < MIN_SPEED){
                _speed = 0f;
                _stopped = true;
            }
        }
    }
}
