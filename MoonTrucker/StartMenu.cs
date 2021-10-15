using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;
using MoonTrucker.GameWorld;

namespace MoonTrucker
{
    public class StartMenu: GameWorld.IDrawable
    {
        private SpriteBatch _spriteBatch;
        private float _screenWidthPx;
        private float _screenHeightPx;
        private SpriteFont _font;

        public StartMenu(float screenWidthPx, float screenHeightPx, SpriteFont font, SpriteBatch spriteBatch)
        {
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _font = font;
            _spriteBatch = spriteBatch;
        }

        public void Draw()
        {
            // TODO: Figure out length of text and use that to get width pos instead of magic number of 0.4f
            var messagePosition = new Vector2(_screenWidthPx * 0.4f, _screenHeightPx * (1 / 3f));
            _spriteBatch.DrawString(_font, "Street Racer", messagePosition, Color.Red);
        }

        public void Update()
        {

        }
    }
}
