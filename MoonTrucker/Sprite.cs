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
        public abstract Color ImageColor { get; }
        internal GameContent _gamecontent;
        internal SpriteBatch _spriteBatch;

        public Sprite(GameContent content, SpriteBatch batch)
        {
            _gamecontent = content;
            _spriteBatch = batch;
        }

        public float Height => Image.Height * ImageScale;
        public float Width => Image.Width * ImageScale;

        public void Draw(float x, float y)
        {
            _spriteBatch.Draw(Image, new Vector2(x, y), null, ImageColor, 0, new Vector2(0, 0), ImageScale, SpriteEffects.None, 0);
        }
    }

    public sealed class TruckSprite : Sprite
    {
        public override Texture2D Image => _gamecontent.ImgTruck;
        public override float ImageScale => 0.25f;
        public override Color ImageColor => Color.MediumPurple;

        public TruckSprite(GameContent content, SpriteBatch batch) : base(content, batch) { }
    }

    public sealed class CarSprite : Sprite
    {
        public override Texture2D Image => _gamecontent.ImgViperCar;
        public override float ImageScale => 0.25f;
        public override Color ImageColor => Color.White;

        public CarSprite(GameContent content, SpriteBatch batch) : base(content, batch) { }
    }
}