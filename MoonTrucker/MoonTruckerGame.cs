using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonTrucker.GameWorld;
using System;
using MoonTrucker.Core;
using Microsoft.Xna.Framework.Media;

namespace MoonTrucker
{
    public enum GameMode
    {
        Arcade,
        Endless,
        Debug
    }

    public class MoonTruckerGame : Game
    {
        private LevelConfig[] _debug = new LevelConfig[1]
        {
            new LevelConfig(int.MaxValue, (/* useSolidDebug */ false) ? "MoonTrucker.GameWorld.Maps.TestBench_.txt" : "MoonTrucker.GameWorld.Maps.TestBench.txt", Color.DarkSalmon, 10)
        };
        private LevelConfig[] _arcadeLevels = new LevelConfig[2]
        {
            //new LevelConfig(15, "MoonTrucker.GameWorld.Maps.Level.txt")
            new LevelConfig(60, "MoonTrucker.GameWorld.Maps.ArcadeMode.Level3.txt", Color.Aqua),
            new LevelConfig(60, "MoonTrucker.GameWorld.Maps.ArcadeMode.Level2.txt", Color.MediumPurple)
        };

        private LevelConfig[] _endlessLevel = new LevelConfig[1]
        {
            new LevelConfig(15, "MoonTrucker.GameWorld.Maps.Map.txt", Color.Aqua, 10)
        };

        private const bool _fullScreen = false;
        private const int _resolutionWidthPx = 1920;
        private const int _resolutionHeightPx = 1080;

        private SpriteFont _font;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int _screenWidthPx;
        private int _screenHeightPx;
        private KeyboardState _oldKeyboardState;
        private TextureManager _textureManager;

        private ResolutionIndependentRenderer _independentRenderer;
        private GameState _gameState = GameState.StartMenu;
        private MainGame _mainGame;
        private StartMenu _startMenu;
        private HUD _gameHUD;

        private Song _gameMusic;
        private Song _menuMusic;


        public MoonTruckerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            setScreenDimensions();
            _independentRenderer = new ResolutionIndependentRenderer(this);
            initializeResolutionIndependence(_screenWidthPx, _screenHeightPx);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            try
            {
                _font = Content.Load<SpriteFont>("Fonts/NoSurrender");
            }
            catch (Exception)
            {
                _font = Content.Load<SpriteFont>("Fonts/Basic");
            }
            _gameMusic = Content.Load<Song>("Sounds/GameBackgroundMusic");
            _menuMusic = Content.Load<Song>("Sounds/TitleMenuMusic");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _textureManager = new TextureManager(Content, GraphicsDevice);


            //create game objects
            _mainGame = new MainGame(_textureManager, _spriteBatch, _independentRenderer, _gameMusic);
            _startMenu = new StartMenu(_screenWidthPx, _screenHeightPx, _font, _spriteBatch, _textureManager, _menuMusic);
            _gameHUD = new HUD(_mainGame, _spriteBatch, _font, _textureManager, _screenWidthPx, _screenHeightPx, _independentRenderer);
        }

        private void initializeResolutionIndependence(int realScreenWidth, int realScreenHeight)
        {
            _independentRenderer.VirtualWidth = _resolutionWidthPx;
            _independentRenderer.VirtualHeight = _resolutionHeightPx;
            _independentRenderer.ScreenWidth = realScreenWidth;
            _independentRenderer.ScreenHeight = realScreenHeight;
            _independentRenderer.Initialize();
        }

        private void setScreenDimensions()
        {
            _screenWidthPx = _fullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : 1600;
            _screenHeightPx = _fullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : 900;

            _graphics.PreferredBackBufferWidth = _screenWidthPx;
            _graphics.PreferredBackBufferHeight = _screenHeightPx;
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState newKeyboardState = Keyboard.GetState();
            var wasEnteredPressed = InputHelper.WasKeyPressed(Keys.Enter, newKeyboardState, _oldKeyboardState);
            _gameHUD.Update(_gameState, newKeyboardState, _oldKeyboardState); // HUD must update before any other actions that could change state
            if (_gameState == GameState.Playing)
            {
                _mainGame.Update(gameTime, newKeyboardState);
                if (_mainGame.IsGameOver())
                {
                    _gameState = GameState.GameOver;
                }
            }
            else if (_gameState == GameState.GameOver)
            {
                MediaPlayer.Stop();
                if (wasEnteredPressed)
                {
                    _gameState = GameState.StartMenu;
                    _startMenu.InitializeStartMenu();
                }
            }
            else if (_gameState == GameState.StartMenu)
            {
                _startMenu.Update(newKeyboardState, _oldKeyboardState, gameTime);
                if (_startMenu.ShouldStart)
                {
                    startGame();
                    _startMenu.AcknowledgeStartGame();
                }
            }

            _oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        private void startGame()
        {
            var gameMode = _startMenu.SelectedGameMode;
            var levels = (gameMode == GameMode.Arcade) ? _arcadeLevels :
                         (gameMode == GameMode.Debug) ? _debug :
                         _endlessLevel;
            _mainGame.SetMode(gameMode);
            _mainGame.StartGame(levels);
            _gameState = GameState.Playing;
        }

        protected override void Draw(GameTime gameTime)
        {
            _independentRenderer.BeginDraw();
            if (_gameState != GameState.StartMenu)
            {
                // main game handles spriteBatch
                _mainGame.Draw();
            }

            _spriteBatch.Begin();
            if (_gameState == GameState.StartMenu)
            {
                _startMenu.Draw(_gameHUD.GetHighScoresForMode(_startMenu.HighScoreMode));
            }
            else
            {
                _gameHUD.Draw(_gameState);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
