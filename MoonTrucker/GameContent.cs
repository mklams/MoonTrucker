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

        public GameContent(ContentManager content)
        {
            //load images
            ImgViperCar = content.Load<Texture2D>("Black_viper");
            ImgTruck = content.Load<Texture2D>("truck");
            ImgCircle = content.Load<Texture2D>("CircleSprite");
        }
    }
}
