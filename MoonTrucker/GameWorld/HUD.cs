using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonTrucker.Core;
using MoonTrucker.Vehicle;

namespace MoonTrucker.GameWorld
{
    public class HUD
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _arrow;
        private float _screenWidthPx;
        private float _screenHeightPx;
        private ResolutionIndependentRenderer _independentRenderer;
        private MainGame _game;

        //TODO: Break out getting high score initials to own class
        private string _highScoreName;

        public HUD(MainGame game, SpriteBatch spriteBatch, SpriteFont font, TextureManager textureManager, float screenWidthPx, float screenHeightPx, ResolutionIndependentRenderer renderer)
        {
            _game = game;
            _spriteBatch = spriteBatch;
            _font = font;
            _arrow = textureManager.GetTexture("Arrow");
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _independentRenderer = renderer;
        }

        public void Update(GameState gameState, GameSave game, KeyboardState newKeyboardState)
        {
            if(gameState == GameState.GameOver)
            {
                // When game is over, if user has a top 10 score let them entre initials
                

            }
        }


        public void Draw(GameState state)
        {
            drawScore();
            drawTimer();
            drawArrow();
            if (state == GameState.GameOver)
            {
                drawGameOver();
            }
        }

        private void drawScore()
        {
            var scorePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(5, 0));
            _spriteBatch.DrawString(_font, $"Score: {_game.GetScore()}", scorePosition, Color.Red);
        }

        private void drawTimer()
        {
            var timePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(200, 0));

            _spriteBatch.DrawString(_font, $"Countdown: {_game.GetTimeLeft()}", timePosition, Color.Red);
        }

        private void drawGameOver()
        {
            //var messagePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(_screenWidthPx / 2f, _screenHeightPx / 2f));
            // TODO: Figure out length of text and use that to get width pos instead of magic number of 0.4f
            var messagePosition = new Vector2(_screenWidthPx * 0.4f, _screenHeightPx * (1 / 3f));

            _spriteBatch.DrawString(_font, "Game Over", messagePosition, Color.Red);
        }
        private void drawArrow()
        {
            var arrowPosition = new Vector2(_screenWidthPx / 2f, 70);
            var arrowCenter = new Vector2(_arrow.Width / 2f, _arrow.Height / 2f);
            _spriteBatch.Draw(_arrow, arrowPosition, null, Color.White, _game.GetAngleFromVehicleToTarget(), arrowCenter, new Vector2(.15f, .15f), SpriteEffects.None, 1f);
        }
    }
}
