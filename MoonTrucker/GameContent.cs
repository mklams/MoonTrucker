﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MoonTrucker
{
    public class GameContent
    {
        public Texture2D ImgViperCar { get; set; }
        public Texture2D ImgTruck { get; set; }
        public Texture2D ImgCircle { get; set; }
        public Texture2D ImgGround {get; set; }
        public Texture2D SolidRectangle { get; set; }

        public GameContent(ContentManager content, GraphicsDevice graphicsdevice)
        {
            //load images
            ImgViperCar = content.Load<Texture2D>("Black_viper");
            ImgTruck = content.Load<Texture2D>("truck");
            ImgCircle = content.Load<Texture2D>("CircleSprite");
            ImgGround = content.Load<Texture2D>("GroundSprite");
            SolidRectangle = new Texture2D(graphicsdevice, 1, 1);
            SolidRectangle.SetData(new[]{Color.White});
        }
    }
}
