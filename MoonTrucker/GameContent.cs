using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MoonTrucker
{
    public class GameContent
    {
        public Texture2D ImgViperCar { get; set; }

        public GameContent(ContentManager content)
        {
            //load images
            ImgViperCar = content.Load<Texture2D>("Black_viper");
        }
    }
}
