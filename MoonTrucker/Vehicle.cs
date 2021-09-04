using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class Vehicle
    {
        public float X { get; private set; } 
        public float Y { get; private set; }
        private readonly float _screenWidth;
        private readonly float _screenHeight;

        private Sprite _sprite { get; set; }

        public Vehicle(Sprite vehicleSprite, float x, float y, float screenWidth, float screenHeight)
        {
            _sprite = vehicleSprite;
            X = x;
            Y = y;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void Draw()
        {
            _sprite.Draw(X, Y);
        }

        public void MoveUp()
        {
            Y = Y - 5;
            if (Y < 1)
            {
                Y = 1;
            }
        }
        public void MoveDown()
        {
            Y = Y + 5;
            if ((Y + _sprite.Height) > _screenHeight)
            {
                Y = _screenHeight - _sprite.Height;
            }
        }

        public void MoveTo(float x)
        {
            if (x >= 0)
            {
                if (x < _screenHeight - _sprite.Height)
                {
                    X = x;
                }
                else
                {
                    X = _screenHeight - _sprite.Height;
                }
            }
            else
            {
                if (x < 0)
                {
                    X = 0;
                }
            }
        }
    }
}
