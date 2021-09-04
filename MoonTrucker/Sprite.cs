using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public abstract class Sprite
    {
        public abstract Texture2D Image { get; }
        public abstract float ImageScale { get; }

        public float Height => Image.Height * ImageScale;
        public float Width => Image.Width * ImageScale;
    }

    public sealed class TruckSprite : Sprite
    {
        public TruckSprite(GameContent gameContent)
        {
            content = gameContent;
        }

        private GameContent content;
        public override Texture2D Image => content.ImgTruck;
        public override float ImageScale => 0.25f;
    }

    public sealed class CarSprite : Sprite
    {
        public CarSprite(GameContent gameContent)
        {
            content = gameContent;
        }

        private GameContent content;
        public override Texture2D Image => content.ImgViperCar;
        public override float ImageScale => 0.25f;
    }
}