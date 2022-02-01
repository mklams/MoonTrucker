using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public StartMenu(float screenWidthPx, float screenHeightPx, SpriteFont font, SpriteBatch spriteBatch)
        {
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _font = font;
            _spriteBatch = spriteBatch;
        }

        public void Draw(HighScores scores)
        {
            var gameName = "Street Racer";
            var messagePosition = new Vector2(getCenterXPositionForText(gameName), _screenHeightPx * (1 / 4f));
            _spriteBatch.DrawString(_font, gameName, messagePosition, Color.Red);

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

        public void Update()
        {

        }

        public float getCenterXPositionForText(string text)
        {
            var messageWidth = _font.MeasureString(text).X;
            return _screenWidthPx * 0.5f - messageWidth * 0.5f;
        }
    }
}
