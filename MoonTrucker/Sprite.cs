using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public abstract class Sprite
    {
        public abstract Texture2D Image { get; }
        public virtual float ImageScale => 1f;
        public virtual Color ImageColor => Color.White;
        internal GameContent _gamecontent;
        internal SpriteBatch _spriteBatch;

        public Sprite(GameContent content, SpriteBatch batch)
        {
            _gamecontent = content;
            _spriteBatch = batch;
        }

        public virtual float Height => Image.Height * ImageScale;
        public virtual float Width => Image.Width * ImageScale;

        public virtual void Draw(Vector2 orgin, float direction)
        {
            _spriteBatch.Draw(Image, orgin, null, ImageColor, direction, new Vector2(Image.Width / 2, Image.Height / 2), ImageScale, SpriteEffects.None, 0);
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

        public CarSprite(GameContent content, SpriteBatch batch) : base(content, batch) { }
    }

    public sealed class CircleSprite : Sprite
    {
        public override Texture2D Image => _gamecontent.ImgCircle;

        public CircleSprite(GameContent content, SpriteBatch batch) : base(content, batch) { }
    }

    public sealed class WallSprite : Sprite
    {
        public override Texture2D Image => _gamecontent.ImgGround;

        public WallSprite(GameContent content, SpriteBatch batch) : base(content, batch) { }
    }

    public sealed class RectangleSprite : Sprite
    {
        public override Texture2D Image => _gamecontent.SolidRectangle;
        public override float Height => _height * ImageScale;
        public override float Width => _width * ImageScale;
        public override Color ImageColor { get; }

        private float _height;
        private float _width;

        public RectangleSprite(GameContent content, SpriteBatch batch) : base(content, batch) {
            ImageColor = Color.Black;
            _height = 1;
            _width = 1;
        }
        public RectangleSprite(GameContent content, SpriteBatch batch, Color color, float width, float height) : base(content, batch) {
            ImageColor = color;
            _height = height;
            _width = width;
        }

        public override void Draw(Vector2 orgin, float direction)
        {
            _spriteBatch.Draw(_gamecontent.SolidRectangle, orgin, null,
            ImageColor, direction, Vector2.Zero, new Vector2(Width, Height), SpriteEffects.None, 0f);
        }
    }
}