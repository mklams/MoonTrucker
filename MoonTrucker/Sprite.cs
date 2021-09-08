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

        public void Draw(Vector2 position, float direction)
        {
            _spriteBatch.Draw(Image, position, null, ImageColor, direction, new Vector2(Image.Width / 2, Image.Height / 2), ImageScale, SpriteEffects.None, 0);
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

    public sealed class CircleSprite : Sprite
    {
        public override Texture2D Image => _gamecontent.ImgCircle;
        public override float ImageScale => 1f;
        public override Color ImageColor => Color.White;

        public CircleSprite(GameContent content, SpriteBatch batch) : base(content, batch) { }
    }

    public sealed class WallSprite : Sprite
    {
        public override Texture2D Image => _gamecontent.ImgGround;
        public override float ImageScale => 1f;
        public override Color ImageColor => Color.White;

        public WallSprite(GameContent content, SpriteBatch batch) : base(content, batch) { }
    }
}