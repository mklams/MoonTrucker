using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MoonTrucker.Core;

namespace MoonTrucker
{
    // TODO: High Scores should be moved to their own class. Maybe under a new subfolder called Screens
    public class StartMenu
    {

        public bool ShouldStart { get; private set; }
        private enum MenuOptions
        {
            Play,
            HighScores
        }
        private MenuOptions _selectedOption;
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
        private int _numberOfRacingParticles = 35;
        private Direction _lastGeneratedDirection = Direction.Vertical;
        private List<LinearParticleTrail> _racingParticles;
        private Random rand = new Random();
        private double _minTimeBetweenRacingParticles = 0.15;
        private double _lastRacingParticleCreationTime = 0.0;
        private GameMode _highScoreMode = GameMode.Arcade;
        private GameMode _selectedGameMode = GameMode.Arcade;
        private Texture2D _arrowRight;
        private Texture2D _arrowLeft;

        public GameMode HighScoreMode => _highScoreMode;
        public GameMode SelectedGameMode => _selectedGameMode;

        private Texture2D _pixel;
        private Song _menuMusic;
        private Color _baseColor = Color.Silver;
        public StartMenu(float screenWidthPx, float screenHeightPx, SpriteFont font, SpriteBatch spriteBatch, TextureManager textureManager, Song backgroundMusic)
        {
            ShouldStart = false;
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _textureManager = textureManager;
            _font = font;
            _spriteBatch = spriteBatch;
            _selectedOption = MenuOptions.Play;
            _pixel = _textureManager.GetTexture("pixel");
            _arrowRight = _textureManager.GetTexture("ArrowRight");
            _arrowLeft = _textureManager.GetTexture("ArrowLeft");
            changeTextureColor(_arrowRight, Color.White);
            _racingParticles = new List<LinearParticleTrail>();
            _menuMusic = backgroundMusic;
            InitializeStartMenu();
        }

        //Called by owner of StartMenu. 
        public void AcknowledgeStartGame()
        {
            ShouldStart = false;
            _showHighScores = false;
        }

        public void InitializeStartMenu()
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(_menuMusic);
            MediaPlayer.IsRepeating = true;
            _racingParticles = new List<LinearParticleTrail>();
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
            drawRacingParticles();
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
            var spacing = _font.LineSpacing * _highScoreNameScale;
            var highScoreMessage = $"High Scores - {getModeText(HighScoreMode)}";
            var messagePosition = new Vector2(getCenterXPositionForText(highScoreMessage, _highScoreTitleScale), _screenHeightPx * (1 / 4f));

            var leftArrowPos = new Vector2(messagePosition.X - 100f, messagePosition.Y);
            drawMenuArrow(leftArrowPos);

            var rightArrowPos = new Vector2(messagePosition.X + (_font.MeasureString(highScoreMessage).X * _highScoreTitleScale), messagePosition.Y);
            drawMenuArrow(rightArrowPos);

            _spriteBatch.DrawString(_font, highScoreMessage, messagePosition, _baseColor, 0f, Vector2.Zero, _highScoreTitleScale, SpriteEffects.None, 1);
            
            var scoreYPosition = (messagePosition.Y + 2 * spacing);
            foreach (Score score in scores.GetTopScores())
            {
                scoreYPosition += spacing;
                var scoreMessage = $"{score.Name}    {score.HitTotal}";
                _spriteBatch.DrawString(_font, scoreMessage, new Vector2(getCenterXPositionForText(scoreMessage, _highScoreNameScale), scoreYPosition), _baseColor, 0, Vector2.Zero, _highScoreNameScale, SpriteEffects.None, 1);
            }
        }

        private void drawMenuArrow(Vector2 position)
        {
            //_spriteBatch.Draw(_menuArrow, new Rectangle((int)position.X, (int)position.Y, 150, 150), Color.White);
        }

        private void drawTitleLogo()
        {
            var gameName = "Street Racer";
            var messagePosition = new Vector2(getCenterXPositionForText(gameName, _titleScale), _screenHeightPx * (1 / 4f));
            _spriteBatch.DrawString(_font, gameName, messagePosition, _baseColor, 0f, Vector2.Zero, _titleScale, SpriteEffects.None, 1);
        }

        private void drawMenuOptions()
        {
            var spacing = _font.LineSpacing * _menuScale;
            var menuYPos = _screenHeightPx * (2 / 3f);
            foreach (MenuOptions option in Enum.GetValues(typeof(MenuOptions)))
            {
                var menuMessage = this.getMenuOptionText(option);
                if (_selectedOption == option)
                {
                    _spriteBatch.DrawString(_font, menuMessage, new Vector2(getCenterXPositionForText(menuMessage, _menuScale), menuYPos), _baseColor, 0f, Vector2.Zero, _menuScale, SpriteEffects.None, 1);
                    _spriteBatch.Draw(_pixel, new Rectangle((int)getCenterXPositionForText(menuMessage, _menuScale), (int)(menuYPos + (_font.MeasureString(menuMessage).Y * _menuScale) - 10), (int)(_font.MeasureString(menuMessage).X * _menuScale), 5), _baseColor);
                    menuYPos += spacing;
                }
                else
                {
                    _spriteBatch.DrawString(_font, menuMessage, new Vector2(getCenterXPositionForText(menuMessage, _menuScale), menuYPos), _baseColor, 0f, Vector2.Zero, _menuScale, SpriteEffects.None, 1);
                    menuYPos += spacing;
                }
            }
        }

        public void Update(KeyboardState keyboardState, KeyboardState oldKeyboardState, GameTime gameTime)
        {
            this.updateRacingParticles(gameTime);
            if (_showHighScores)
            {
                if (InputHelper.WasKeyPressed(Keys.Enter, keyboardState, oldKeyboardState)
                || InputHelper.WasKeyPressed(Keys.Space, keyboardState, oldKeyboardState)
                || InputHelper.WasKeyPressed(Keys.Escape, keyboardState, oldKeyboardState))
                {
                    _showHighScores = false;
                }

                if (InputHelper.WasKeyPressed(Keys.Left, keyboardState, oldKeyboardState)
                    || InputHelper.WasKeyPressed(Keys.Right, keyboardState, oldKeyboardState))
                {
                    _highScoreMode = (_highScoreMode == GameMode.Arcade) ? GameMode.Endless : GameMode.Arcade;
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
                    if (_selectedOption == MenuOptions.Play)
                    {
                        ShouldStart = true;
                    }
                    if (_selectedOption == MenuOptions.HighScores)
                    {
                        _showHighScores = true;
                    }
                }else if (InputHelper.WasKeyPressed(Keys.Right, keyboardState, oldKeyboardState)
                    || InputHelper.WasKeyPressed(Keys.Left, keyboardState, oldKeyboardState))
                {
                    _selectedGameMode = getNextMode(_selectedGameMode);
                }
            }
            this.updateColorFade();
        }

        private GameMode getNextMode(GameMode mode)
        {
            // With LINQ as your hammer, the world is full of nails
            var lastMode = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().Last();
            if(mode == lastMode)
            {
                return Enum.GetValues(typeof(GameMode)).Cast<GameMode>().First();
            }

            return Enum.GetValues(typeof(GameMode)).Cast<GameMode>()
                .SkipWhile(e => e != mode).Skip(1).First();

        }

        private void navigateBackwardsInMenu()
        {
            _selectedOption--;
            if (_selectedOption < 0)
            {
                _selectedOption = Enum.GetValues(typeof(MenuOptions)).Cast<MenuOptions>().Max();
            }
        }

        private void navigateForwardsInMenu()
        {
            _selectedOption++;
            if ((int)_selectedOption >= Enum.GetNames(typeof(MenuOptions)).Length)
            {
                _selectedOption = Enum.GetValues(typeof(MenuOptions)).Cast<MenuOptions>().Min();
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

        private string getModeText(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Arcade:
                    return "Arcade";
                case GameMode.Endless:
                    return "Endless";
                case GameMode.Debug:
                    return "Debug";
                default:
                    return "";
            }
        }

        private string getMenuOptionText(MenuOptions option)
        {
            switch (option)
            {
                case MenuOptions.Play:
                    return getModeText(_selectedGameMode);
                case MenuOptions.HighScores:
                    return "High Scores";
                default:
                    return "No Text Found!";
            }
        }

        private void drawRacingParticles()
        {
            _racingParticles.ForEach(lpt => lpt.Draw());
        }

        private void updateRacingParticles(GameTime gameTime)
        {
            if (_racingParticles.Count < _numberOfRacingParticles && gameTime.TotalGameTime.TotalSeconds - _lastRacingParticleCreationTime > _minTimeBetweenRacingParticles)
            {
                _lastRacingParticleCreationTime = gameTime.TotalGameTime.TotalSeconds;
                if (_lastGeneratedDirection == Direction.Horizontal)
                {
                    _racingParticles.Add(
                        new LinearParticleTrail(
                            new Vector2(_screenWidthPx, _screenHeightPx),
                            Direction.Vertical,
                            rand.Next(15, ((int)_screenWidthPx - 15)),
                            getColor(),
                            _spriteBatch,
                            _textureManager
                        )
                    );
                    _lastGeneratedDirection = Direction.Vertical;
                }
                else
                {
                    _racingParticles.Add(
                        new LinearParticleTrail(
                            new Vector2(_screenWidthPx, _screenHeightPx),
                            Direction.Horizontal,
                            rand.Next(15, ((int)_screenHeightPx - 15)),
                            getColor(),
                            _spriteBatch,
                            _textureManager
                        )
                    );
                    _lastGeneratedDirection = Direction.Horizontal;
                }
            }
            List<LinearParticleTrail> toRemove = new List<LinearParticleTrail>();
            _racingParticles.ForEach((lpt) =>
            {
                if (lpt.IsDone())
                {
                    toRemove.Add(lpt);
                }
                else
                {
                    lpt.Update(gameTime);
                }
            });
            toRemove.ForEach(lpt => _racingParticles.Remove(lpt));
        }

        private void changeTextureColor(Texture2D texture, Color color)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != Color.Transparent)
                {
                    data[i] = color;
                }
            }
            texture.SetData(data);
        }
    }
}
