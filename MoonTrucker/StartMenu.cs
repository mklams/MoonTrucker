using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonTrucker.Core;
using MoonTrucker.GameWorld;

namespace MoonTrucker
{
    public class StartMenu
    {

        public bool ShouldStart { get; private set; }
        private enum MenuOptions
        {
            Start,
            HighScores
        }
        private MenuOptions _selectedOption;
        private List<MenuOptions> _options;
        private SpriteBatch _spriteBatch;

        private TextureManager _textureManager;
        private float _screenWidthPx;
        private float _screenHeightPx;
        private SpriteFont _font;
        private bool _showHighScores = false;
        private float _percentFade = 0f;
        private float _deltaFade = 0.005f;
        private Color _startColor = Color.Red;
        private Color _endColor = Color.Blue;
        private float _titleScale = 1f;
        private float _menuScale = 0.5f;
        private float _highScoreTitleScale = 0.75f;
        private float _highScoreNameScale = 0.25f;

        private Texture2D _pixel;
        public StartMenu(float screenWidthPx, float screenHeightPx, SpriteFont font, SpriteBatch spriteBatch, TextureManager textureManager)
        {
            ShouldStart = false;
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _textureManager = textureManager;
            _font = font;
            _spriteBatch = spriteBatch;
            _options = new List<MenuOptions>() { MenuOptions.Start, MenuOptions.HighScores };
            _selectedOption = MenuOptions.Start;
            _pixel = _textureManager.GetTexture("pixel");
        }

        //Called by owner of StartMenu. 
        public void AcknowledgeStartGame()
        {
            ShouldStart = false;
            _showHighScores = false;
        }

        private Color getColor()
        {
            var diffRed = _endColor.R - _startColor.R;
            var diffGreen = _endColor.G - _startColor.G;
            var diffBlue = _endColor.B - _startColor.B;

            float newRed = (diffRed * _percentFade) + _startColor.R;
            float newGreen = (diffGreen * _percentFade) + _startColor.G;
            float newBlue = (diffBlue * _percentFade) + _startColor.B;

            return new Color((int)Math.Round(newRed), (int)Math.Round(newGreen), (int)Math.Round(newBlue));
        }

        public void Draw(HighScores scores)
        {
            if (!_showHighScores)
            {
                drawTitleLogo();
                drawMenuOptions();
            }
            else
            {
                drawHighScores(scores);
            }
        }

        private void drawHighScores(HighScores scores)
        {
            var spacing = _font.LineSpacing* _highScoreNameScale;
            var highScoreMessage = "High Scores";
            var messagePosition = new Vector2(getCenterXPositionForText(highScoreMessage, _highScoreTitleScale), _screenHeightPx * (1 / 4f));
            _spriteBatch.DrawString(_font, highScoreMessage, messagePosition, Color.Red, 0f, Vector2.Zero, _highScoreTitleScale, SpriteEffects.None, 1);
            var scoreYPosition = (messagePosition.Y + 2 * spacing);
            foreach (Score score in scores.GetTopScores())
            {
                scoreYPosition += spacing;
                var scoreMessage = $"{score.Name}    {score.HitTotal}";
                _spriteBatch.DrawString(_font, scoreMessage, new Vector2(getCenterXPositionForText(scoreMessage, _highScoreNameScale), scoreYPosition), Color.Red,0,Vector2.Zero, _highScoreNameScale, SpriteEffects.None, 1);
            }
        }

        private void drawTitleLogo()
        {
            var gameName = "Street Racer";
            var messagePosition = new Vector2(getCenterXPositionForText(gameName, _titleScale), _screenHeightPx * (1 / 4f));
            _spriteBatch.DrawString(_font, gameName, messagePosition, getColor(), 0f, Vector2.Zero, _titleScale, SpriteEffects.None, 1);
        }

        private void drawMenuOptions()
        {
            var spacing = _font.LineSpacing * _menuScale;
            var menuYPos = _screenHeightPx * (2 / 3f);
            foreach (MenuOptions option in _options)
            {
                var menuMessage = this.getMenuOptionText(option);
                if (_selectedOption == option)
                {
                    _spriteBatch.DrawString(_font, menuMessage, new Vector2(getCenterXPositionForText(menuMessage, _menuScale), menuYPos), getColor(), 0f, Vector2.Zero, _menuScale, SpriteEffects.None, 1);
                    _spriteBatch.Draw(_pixel, new Rectangle((int)getCenterXPositionForText(menuMessage, _menuScale), (int)(menuYPos + (_font.MeasureString(menuMessage).Y * _menuScale) - 10), (int)(_font.MeasureString(menuMessage).X * _menuScale), 5), getColor());
                    menuYPos += spacing;
                }
                else
                {
                    _spriteBatch.DrawString(_font, menuMessage, new Vector2(getCenterXPositionForText(menuMessage, _menuScale), menuYPos), getColor(), 0f, Vector2.Zero, _menuScale, SpriteEffects.None, 1);
                    menuYPos += spacing;
                }
            }
        }

        public void Update(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            if (_showHighScores)
            {
                if (InputHelper.WasKeyPressed(Keys.Enter, keyboardState, oldKeyboardState)
                || InputHelper.WasKeyPressed(Keys.Space, keyboardState, oldKeyboardState)
                || InputHelper.WasKeyPressed(Keys.Escape, keyboardState, oldKeyboardState))
                {
                    _showHighScores = false;
                }
            }
            else
            {
                if (InputHelper.WasKeyPressed(Keys.Up, keyboardState, oldKeyboardState)
                || InputHelper.WasKeyPressed(Keys.W, keyboardState, oldKeyboardState))
                {
                    navigateBackwardsInMenu();
                }
                else if (InputHelper.WasKeyPressed(Keys.Down, keyboardState, oldKeyboardState)
                || InputHelper.WasKeyPressed(Keys.S, keyboardState, oldKeyboardState))
                {
                    navigateForwardsInMenu();
                }
                else if (InputHelper.WasKeyPressed(Keys.Space, keyboardState, oldKeyboardState)
                || InputHelper.WasKeyPressed(Keys.Enter, keyboardState, oldKeyboardState))
                {
                    if (_selectedOption == MenuOptions.Start)
                    {
                        ShouldStart = true;
                    }
                    if (_selectedOption == MenuOptions.HighScores)
                    {
                        _showHighScores = true;
                    }
                }
            }
            this.updateColorFade();
        }

        private void navigateBackwardsInMenu()
        {
            var index = _options.IndexOf(_selectedOption);
            if (index == 0)
            {
                _selectedOption = _options[_options.Count - 1];
            }
            else
            {
                _selectedOption = _options[index - 1];
            }
        }

        private void navigateForwardsInMenu()
        {
            var index = _options.IndexOf(_selectedOption);
            if (index == _options.Count - 1)
            {
                _selectedOption = _options[0];
            }
            else
            {
                _selectedOption = _options[index + 1];
            }
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

        public float getCenterXPositionForText(string text, float scale = 1)
        {
            var messageWidth = _font.MeasureString(text).X;
            messageWidth *= scale;
            return _screenWidthPx * 0.5f - messageWidth * 0.5f;
        }

        private string getMenuOptionText(MenuOptions option)
        {
            switch (option)
            {
                case MenuOptions.Start:
                    return "Start Game";
                case MenuOptions.HighScores:
                    return "High Scores";
                default:
                    return "No Text Found!";
            }
        }
    }
}
