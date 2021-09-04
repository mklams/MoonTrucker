using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class Car
    {
        public float X { get; set; } //x position of paddle on screen
        public float Y { get; set; } //y position of paddle on screen
        public float Width { get; set; } //width of paddle
        public float Height { get; set; } //height of paddle
        public float ScreenWidth { get; set; } //width of game screen
        public float ScreenHeight { get; set; } //width of game screen

        private Texture2D imgViporCar { get; set; }
        private float imgScale { get; set; }
        private SpriteBatch spriteBatch { get; set; }

        public Car(float x, float y, float screenWidth, float screenHeight, SpriteBatch spriteBatch, GameContent gameContent)
        {
            X = x;
            Y = y;
            imgViporCar = gameContent.ImgViperCar;
            imgScale = 0.25f;
            Width = imgViporCar.Width*imgScale;
            Height = imgViporCar.Height*imgScale;
            this.spriteBatch = spriteBatch;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public void Draw()
        {
            spriteBatch.Draw(imgViporCar, new Vector2(X, Y), null, Color.White, 0, new Vector2(0, 0), imgScale * 1.0f, SpriteEffects.None, 0);
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
            if ((Y + Height) > ScreenHeight)
            {
                Y = ScreenHeight - Height;
            }
        }

        public void MoveTo(float x)
        {
            if (x >= 0)
            {
                if (x < ScreenHeight - Height)
                {
                    X = x;
                }
                else
                {
                    X = ScreenHeight - Height;
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
