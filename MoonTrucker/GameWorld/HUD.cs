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
        private GameSave _gameSave; 
        private HighScores _highScores;

        //TODO: Break out getting high score initials to own class
        private string _highScoreName = "";

        public HUD(MainGame game, SpriteBatch spriteBatch, SpriteFont font, TextureManager textureManager, float screenWidthPx, float screenHeightPx, ResolutionIndependentRenderer renderer)
        {
            _gameSave = new GameSave(); // TODO: Should HUD own this or MoonTruckerGame
            _highScores = new HighScores(_gameSave.Load());

            _game = game;
            _spriteBatch = spriteBatch;
            _font = font;
            _arrow = textureManager.GetTexture("Arrow");
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _independentRenderer = renderer;
        }

        public HighScores GetHighScores()
        {
            return _highScores;
        }

        public void Update(GameState gameState, KeyboardState newKeyboardState, KeyboardState oldKeyboardState)
        {
            if(gameState == GameState.GameOver)
            {
                // When game is over, if user has a top 10 score let them entre initials
                if (IsHighScoreGame())
                {

                    if(_highScoreName.Length > 0 && newKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        _highScores.AddScore(new Score(_game.GetScore(), _highScoreName));
                        _gameSave.Save(_highScores.GetTopScores());

                        _highScoreName = "";
                    }

                    if (InputHelper.TryConvertKeyboardInput(newKeyboardState, oldKeyboardState, out char initial))
                    {
                        _highScoreName += initial;
                    }
                }

            }
        }

        private bool IsHighScoreGame()
        {
            return _highScores.IsATopScore(_game.GetScore());
        }

        public void Draw(GameState state)
        {
            drawScore();
            drawTimer();
            drawArrow();
            if (state == GameState.GameOver)
            {
                drawGameOver();
                drawHighScore();
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
            // TODO: Figure out length of text and use that to get width pos instead of magic number of 0.4f
            var messagePosition = new Vector2(_screenWidthPx * 0.4f, _screenHeightPx * (1 / 3f));

            _spriteBatch.DrawString(_font, "Game Over", messagePosition, Color.Red);
        }

        private void drawHighScore()
        {
            if(IsHighScoreGame())
            {
                // TODO: Figure out length of text and use that to get width pos instead of magic number of 0.3f
                var messagePosition = new Vector2(_screenWidthPx * 0.3f, _screenHeightPx * (1 / 2f));
                _spriteBatch.DrawString(_font, $"New High Score! Enter your name: {_highScoreName}", messagePosition, Color.Red);
            }
        }

        private void drawArrow()
        {
            var arrowPosition = new Vector2(_screenWidthPx / 2f, 70);
            var arrowCenter = new Vector2(_arrow.Width / 2f, _arrow.Height / 2f);
            _spriteBatch.Draw(_arrow, arrowPosition, null, Color.White, _game.GetAngleFromVehicleToTarget(), arrowCenter, new Vector2(.15f, .15f), SpriteEffects.None, 1f);
        }
    }
}
