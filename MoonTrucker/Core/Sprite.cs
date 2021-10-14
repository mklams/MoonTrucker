using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonTrucker.Core;

namespace MoonTrucker
{
    public abstract class Sprite
    {
        public abstract Texture2D Image { get; }
        public virtual float ImageScale => 1f;
        public virtual Color ImageColor => Color.White;
        internal TextureManager _textureManager;
        internal SpriteBatch _spriteBatch;

        public Sprite(TextureManager manager, SpriteBatch batch)
        {
            _textureManager = manager;
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
        public override Texture2D Image => _textureManager.GetTexture("truck");
        public override float ImageScale => 0.25f;
        public override Color ImageColor => Color.MediumPurple;

        public TruckSprite(TextureManager manager, SpriteBatch batch) : base(manager, batch) { }
    }

    public sealed class CarSprite : Sprite
    {
        public override Texture2D Image => _textureManager.GetTexture("Black_viper");
        public override float ImageScale => 0.25f;

        public CarSprite(TextureManager manager, SpriteBatch batch) : base(manager, batch) { }
    }

    public sealed class CircleSprite : Sprite
    {
        public override Texture2D Image => _textureManager.GetTexture("CircleSprite");

        public CircleSprite(TextureManager manager, SpriteBatch batch) : base(manager, batch) { }
    }

    public sealed class WallSprite : Sprite
    {
        public override Texture2D Image => _textureManager.GetTexture("GroundSprite");

        public WallSprite(TextureManager manager, SpriteBatch batch) : base(manager, batch) { }
    }
}