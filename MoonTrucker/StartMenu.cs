using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonTrucker.Core;
using MoonTrucker.GameWorld;

namespace MoonTrucker
{
    public class StartMenu
    {
        private SpriteBatch _spriteBatch;
        private float _screenWidthPx;
        private float _screenHeightPx;
        private SpriteFont _font;
        private bool _showHighScores = false;
        private float _percentFade = 0f;
        private float _deltaFade = 0.005f;
        private Color _startColor = Color.Red;
        private Color _endColor = Color.Blue;
        private float _fontScale = 3f;
        public StartMenu(float screenWidthPx, float screenHeightPx, SpriteFont font, SpriteBatch spriteBatch)
        {
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _font = font;
            _spriteBatch = spriteBatch;
        }

        private Color getColor()
        {
            var diffRed = _endColor.R - _startColor.R;
            var diffGreen = _endColor.G - _startColor.G;
            var diffBlue = _endColor.B - _startColor.B;

            float newRed = ((float)diffRed * _percentFade) + (float)_startColor.R;
            float newGreen = ((float)diffGreen * _percentFade) + (float)_startColor.G;
            float newBlue = ((float)diffBlue * _percentFade) + (float)_startColor.B;

            return new Color((int)Math.Round(newRed), (int)Math.Round(newGreen), (int)Math.Round(newBlue));
        }

        public void Draw(HighScores scores)
        {
            var gameName = "Street Racer";
            var messagePosition = new Vector2(getCenterXPositionForText(gameName, true), _screenHeightPx * (1 / 4f));
            _spriteBatch.DrawString(_font, gameName, messagePosition, getColor(), 0f, Vector2.Zero, _fontScale, SpriteEffects.None, 1);

            if (_showHighScores)
            {
                int spacing = _font.LineSpacing + 2;
                var highScoreMessage = "High Scores";
                messagePosition = new Vector2(getCenterXPositionForText(highScoreMessage), _screenHeightPx * (1 / 3f));
                _spriteBatch.DrawString(_font, highScoreMessage, messagePosition, Color.Red);
                var scoreYPosition = messagePosition.Y;
                foreach (Score score in scores.GetTopScores())
                {
                    scoreYPosition += spacing;
                    var scoreMessage = $"{score.Name}    {score.HitTotal}";
                    _spriteBatch.DrawString(_font, scoreMessage, new Vector2(getCenterXPositionForText(scoreMessage), scoreYPosition), Color.Red);
                }
            }


        }

        public void Update(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            if (InputHelper.WasKeyPressed(Keys.Space, keyboardState, oldKeyboardState))
            {
                _showHighScores = !_showHighScores;
            }
            this.updateColorFade();
        }

        private void updateColorFade()
        {
            if (_percentFade >= 1f)
            {
                _percentFade = 0f;
                rotateColors();
                _percentFade -= _deltaFade;
            }
            else
            {
                _percentFade += _deltaFade;
            }
        }

        private void rotateColors()
        {
            if (_startColor == Color.Red)
            {
                _startColor = Color.Blue;
                _endColor = Color.Green;
                return;
            }
            if (_startColor == Color.Blue)
            {
                _startColor = Color.Green;
                _endColor = Color.Red;
                return;
            }
            if (_startColor == Color.Green)
            {
                _startColor = Color.Red;
                _endColor = Color.Blue;
                return;
            }
        }

        public float getCenterXPositionForText(string text, bool isTitle = false)
        {
            var messageWidth = _font.MeasureString(text).X;
            if (isTitle)
            {
                messageWidth *= _fontScale;
            }
            return _screenWidthPx * 0.5f - messageWidth * 0.5f;
        }
    }
}
