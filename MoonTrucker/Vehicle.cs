using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class Vehicle
    {
        public float X { get; set; } 
        public float Y { get; set; } 
        public float ScreenWidth { get; set; } 
        public float ScreenHeight { get; set; } 

        private Sprite sprite { get; set; }

        public Vehicle(Sprite vehicleSprite, float x, float y, float screenWidth, float screenHeight)
        {
            X = x;
            Y = y;
            sprite = vehicleSprite;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public void Draw()
        {
            sprite.Draw(X, Y);
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
            if ((Y + sprite.Height) > ScreenHeight)
            {
                Y = ScreenHeight - sprite.Height;
            }
        }

        public void MoveTo(float x)
        {
            if (x >= 0)
            {
                if (x < ScreenHeight - sprite.Height)
                {
                    X = x;
                }
                else
                {
                    X = ScreenHeight - sprite.Height;
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
