using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class Car
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
        private bool _stopped;
        private const float MAX_SPEED = 8f;
        private const float MIN_SPEED = .9f;
        private const float DEGREES_TO_ROTATE = 5f;

        private long ticksSinceStopping; 
#region Mark's APIs
        /*
         * Mark's Position API
         */
        public Vector2 Position{
            get {return _position;}
        }

        /*
         * Mark's API to stop car. Does not reset ticksSinceStopping.
         * Returns - Vector2 of the position
         */
        public Vector2 Stop()
        {
            _speed = 0f;
            _stopped = true;
            return _position;
        }

        /*
         * Mark's API for sprite width
         */
        public float GetSpriteWidth()
        {
            return _sprite.Width;
        }

        /*
         * Mark's API for sprite height
         */
        public float GetSpriteHeight()
        {
            return _sprite.Height;
        }

#endregion


        public Car(Sprite sprite, Vector2 pos, float screenWidth, float screenHeight)
        {
            _position = pos;
            _speed = 0f;
            _stopped = true;
            _sprite = sprite;
            ticksSinceStopping = 0L;
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
        }

        public void Draw()
        {
            _sprite.Draw(_position, MathHelper.ToRadians(_angle));
        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                handleUpKey(gameTime.TotalGameTime.Ticks - ticksSinceStopping > 10000000L/5L, gameTime);
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                handleDownKey(gameTime.TotalGameTime.Ticks - ticksSinceStopping > 10000000L/5L, gameTime);
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
            if(!_stopped && Math.Abs(_speed) < MIN_SPEED){
                _speed = 0f;
                _stopped = true;
                ticksSinceStopping = gameTime.TotalGameTime.Ticks;
            }
            var clampedPos = Vector2.Clamp(_position, new Vector2(0, 0), new Vector2(_screenWidth, _screenHeight));
            if(clampedPos != _position){
                _stopped = true;
                ticksSinceStopping = gameTime.TotalGameTime.Ticks;
                _speed = 0f;
                _position = clampedPos;
            } 
        }

        private void handleUpKey(bool canStart, GameTime gameTime)
        {
            if(_stopped && canStart)
            {
                _stopped = false;
                _speed = 1f;
            }
            else if(_speed > 0f)
            {
                accelerate();
            }
            else
            {
                decelerate(gameTime);
            }
        }

        private void handleDownKey(bool canStart, GameTime gameTime)
        {
            if(_stopped && canStart)
            {
                _stopped = false;
                _speed = -1f;
            }
            else if(_speed < 0f){
                accelerate();
            }
            else{
                decelerate(gameTime);
            }
        }

        private void handleLeftKey()
        {
            if(_stopped)
            {
                return;
            }
            var degreesToRotate = (_speed > 0f)? -DEGREES_TO_ROTATE : DEGREES_TO_ROTATE;
            _angle += degreesToRotate;
        }

        private void handleRightKey()
        {
            if(_stopped)
            {
                return;
            }
            var degreesToRotate = (_speed > 0f)? DEGREES_TO_ROTATE : -DEGREES_TO_ROTATE;
            _angle += degreesToRotate;
        }

        private void accelerate()
        {
            _speed *= 1.5f;
            if(Math.Abs(_speed) > MAX_SPEED){
                _speed = (_speed > 0f)? MAX_SPEED : -MAX_SPEED;
            }
        }

        private void decelerate(GameTime gameTime)
        {
            _speed*=.9f;
            if(!_stopped && Math.Abs(_speed) < MIN_SPEED){
                _speed = 0f;
                _stopped = true;
                ticksSinceStopping = gameTime.TotalGameTime.Ticks;
            }
        }
    }
}
