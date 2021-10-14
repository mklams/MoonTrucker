using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker.Vehicle
{
    public class VehicleWithMomentum
    {
        private readonly float _screenWidth;
        private readonly float _screenHeight;
        private Vector2 _velocity;
        private float _angle => MathF.Atan(_velocity.Y/_velocity.X);
        private float _lastAngle;
        private Vector2 _position;
        private Sprite _sprite { get; set; }
        private bool _forward;
        private bool _stopped;
        private const float MAX_SPEED = 3f;
        private const float MIN_SPEED = .5f;
        private const float DEGREES_TO_ROTATE = 2f;

        public VehicleWithMomentum(Sprite sprite, Vector2 pos, float screenWidth, float screenHeight)
        {
            _position = pos;
            _velocity = new Vector2(0.001f,0f);
            _forward = true;
            _stopped = true;
            _sprite = sprite;
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
        }

        public void Draw()
        {
            var angle = (_stopped)? _lastAngle : _angle;
            _sprite.Draw(_position, angle);
        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            if(!_stopped)
            {
                _lastAngle = _angle;
            }
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
            _velocity *= .99f;
            var clampedPos = Vector2.Clamp(_position, new Vector2(0, 0), new Vector2(_screenWidth, _screenHeight));
            if(clampedPos != _position){
                _stopped = true;
                _velocity *= .001f;
                _position = clampedPos;
            } 
        }

        private void handleUpKey()
        {
            if(_stopped)
            {
                _stopped = false;
                _forward = true;
                var newVel = new Vector2((float)Math.Cos(_lastAngle), (float)Math.Sin(_lastAngle));
                newVel.Normalize();
                _velocity = newVel;
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
                var newVel = new Vector2(MathF.Cos(_lastAngle), MathF.Sin(_lastAngle));
                newVel.Normalize();
                newVel = newVel * -1f;
                _velocity = newVel;
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
            _velocity = this.rotate(_velocity, degreesToRotate);
            if(!_forward)
            {
                _velocity = -_velocity;
            }
        }

        private void handleRightKey()
        {
            if(_stopped)
            {
                return;
            }
            var degreesToRotate = (_forward)? DEGREES_TO_ROTATE : -DEGREES_TO_ROTATE;
            _velocity = this.rotate(_velocity, degreesToRotate);
            if(!_forward)
            {
                _velocity = -_velocity;
            }
        }

        private Vector2 rotate(Vector2 vector, float degrees) 
        {
            float Vx, Vy;
            Vx = vector.Length()*MathF.Cos(_angle + MathHelper.ToRadians(degrees));
            Vy = vector.Length()*MathF.Sin(_angle + MathHelper.ToRadians(degrees));
            return new Vector2(Vx, Vy);
        }

        private void accelerate()
        {
            var newVel = _velocity *= 1.05f;
            if(newVel.Length() < MAX_SPEED){
                _velocity = newVel;
            }
        }

        private void decelerate()
        {
            var newVel = _velocity *= 0.9f;
            if(newVel.Length() > MIN_SPEED){
                _velocity = newVel;
            }
            else
            {
                _stopped = true;
            }
        }
    }
}
