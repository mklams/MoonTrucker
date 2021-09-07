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
        private const float MIN_SPEED = .5f;
        private const float DEGREES_TO_ROTATE = 5f;
        private const int MILLI_PAUSE_AFTER_STOPPING = 200;

        private int milliSinceStopping; 
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
            milliSinceStopping = 0;
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
                handleUpKey(gameTime.TotalGameTime.Ticks - milliSinceStopping > MILLI_PAUSE_AFTER_STOPPING, gameTime);
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                handleDownKey(gameTime.TotalGameTime.Milliseconds - milliSinceStopping > MILLI_PAUSE_AFTER_STOPPING, gameTime);
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
            _speed *= .97f; //friction
            if(!_stopped && Math.Abs(_speed) < MIN_SPEED){
                _speed = 0f;
                _stopped = true;
                milliSinceStopping = gameTime.TotalGameTime.Milliseconds;
            }
            var clampedPos = Vector2.Clamp(_position, new Vector2(0, 0), new Vector2(_screenWidth, _screenHeight));
            if(clampedPos != _position){
                _stopped = true;
                milliSinceStopping = gameTime.TotalGameTime.Milliseconds;
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
            var allowedRadius = getTurnRadius();
            var degreesToRotate = (_speed > 0f)? -allowedRadius : allowedRadius;
            _angle += degreesToRotate;
        }

        private void handleRightKey()
        {
            if(_stopped)
            {
                return;
            }
            var allowedRadius = getTurnRadius();
            var degreesToRotate = (_speed > 0f)? allowedRadius : -allowedRadius;
            _angle += degreesToRotate;
        }

        private void accelerate()
        {
            _speed *= getAccelerationAmount();
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
                milliSinceStopping = gameTime.TotalGameTime.Milliseconds;
            }
        }

        private float getAccelerationAmount(){
            if(_speed < 0f){
                return 1.05f;
            }
            float speedPercentile = (_speed/MAX_SPEED)*100;
            if(speedPercentile < 25f){
                return (.15f*_speed)+1f;
            }
            else if(speedPercentile < ((35f/80f)*100)){
                return (.1f*_speed)+1.05f;
            }
            else if(speedPercentile < 50f){
                return (-.4f*_speed)+2.9f;
            }
            else if(speedPercentile < 67.5f){
                return (-.2f*_speed)+2.1f;
            }
            else if(speedPercentile < 75){
                return (-.05f*_speed)+1.35f;
            }
            else {
                return (-.03f*_speed)+1.14f;
            }
        }

        private float getTurnRadius(){
            var absSpeed = Math.Abs(_speed);
            var speedPercentile = absSpeed/MAX_SPEED;
            if(speedPercentile < 25){
                return 4f;
            }
            if(speedPercentile < 50){
                return 3f;
            }
            if(speedPercentile < 75){
                return 2f;
            }
            return 1.5f;
        }
    }
}
