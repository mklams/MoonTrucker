using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class TruckSprite
    {
        public TruckSprite(GameContent gameContent)
        {
            content = gameContent;
        }

        private GameContent content;
        public Texture2D Image => content.ImgTruck;
        public float ImageScale = 0.25f;

        public float Height => Image.Height * ImageScale;
        public float Width => Image.Width * ImageScale;

    }
}