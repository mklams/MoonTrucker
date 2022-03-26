using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonTrucker.Core;

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
        private float _hudScale = 0.2f;
        private float _gameOverScale = 0.3f;
        private float _highScoreScale = 0.25f;

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

        public HighScores GetHighScoresForMode(GameMode mode)
        {
            return new HighScores(_highScores.GetTopScoresForMode(mode));
        }

        public void Update(GameState gameState, KeyboardState newKeyboardState, KeyboardState oldKeyboardState)
        {
            if(gameState == GameState.GameOver)
            {
                if (IsHighScoreGame())
                {

                    if(_highScoreName.Length > 0 && newKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        _highScores.AddScore(new Score(_game.GetScore(), _highScoreName, _game.Mode));
                        _gameSave.Save(_highScores.GetTopScores());

                        _highScoreName = "";
                    }

                    // abritrary 21 character limit for name
                    if (InputHelper.TryConvertKeyboardInput(newKeyboardState, oldKeyboardState, out char initial) && _highScoreName.Length < 21)
                    {
                        _highScoreName += initial;
                    }
                }

            }
        }

        private bool IsHighScoreGame()
        {
            return _highScores.IsATopScore(_game.GetScore(), _game.Mode);
        }

        public void Draw(GameState state)
        {
            drawScore();
            drawTimer();
            if(_game.ShowArrow())
            {
                drawArrow();
            }

            if(_game.Mode != GameMode.Endless)
            {
                drawLevel();
            }
            


            if (state == GameState.GameOver)
            {
                drawGameOver();
                drawHighScore();
            }
        }

        private void drawScore()
        {
            var scorePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(5, 0));
            _spriteBatch.DrawString(_font, $"Score: {_game.GetScore()}", scorePosition, Color.Red, 0, Vector2.Zero, _hudScale, SpriteEffects.None, 1);
        }

        private void drawTimer()
        {
            var timePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(200, 0));

            _spriteBatch.DrawString(_font, $"Countdown: {_game.GetTimeLeft()}", timePosition, Color.Red, 0, Vector2.Zero, _hudScale, SpriteEffects.None, 1);
        }

        private void drawLevel()
        {
            var timePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(500, 0));
            _spriteBatch.DrawString(_font, $"Level: {_game.GetCurrentLevelNumber()}", timePosition, Color.Red, 0, Vector2.Zero, _hudScale, SpriteEffects.None, 1);
        }

        private void drawGameOver()
        {
            string message = _game.PlayerWon ? "You Won! You are the raddest street racer!" : "Game Over";
            var messagePosition = new Vector2(getCenterXPositionForText(message, _gameOverScale), _screenHeightPx * (1 / 3f));

            _spriteBatch.DrawString(_font, message, messagePosition, Color.Red, 0, Vector2.Zero, _gameOverScale, SpriteEffects.None, 1);
        }

        private void drawHighScore()
        {
            if(IsHighScoreGame())
            {
                var message = $"New High Score! Enter your name: {_highScoreName}";
                var messagePosition = new Vector2(getCenterXPositionForText(message, _highScoreScale), _screenHeightPx * (1 / 2f));
                _spriteBatch.DrawString(_font, message, messagePosition, Color.Red, 0, Vector2.Zero, _highScoreScale, SpriteEffects.None, 1);
            }
        }

        private void drawArrow()
        {
            var arrowPosition = new Vector2(_screenWidthPx / 2f, 70);
            var arrowCenter = new Vector2(_arrow.Width / 2f, _arrow.Height / 2f);
            var destPosition = _game.GetAngleFromVehicleToDestination();
            _spriteBatch.Draw(_arrow, arrowPosition, null, Color.White, destPosition, arrowCenter, new Vector2(.15f, .15f), SpriteEffects.None, 1f);
        }

        public float getCenterXPositionForText(string text, float scale)
        {
            var messageWidth = _font.MeasureString(text).X* scale;
            return _screenWidthPx * 0.5f - messageWidth * 0.5f;
        }
    }
}
