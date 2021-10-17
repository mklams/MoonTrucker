﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Utilities;
using MoonTrucker.GameWorld;
using System;
using MoonTrucker.Core;

namespace MoonTrucker
{
    public class MoonTruckerGame : Game
    {
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

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _textureManager = new TextureManager(Content, GraphicsDevice);
            

            //create game objects
            _mainGame = new MainGame(getScreenCenter(), _textureManager, _spriteBatch, _independentRenderer);
            _startMenu = new StartMenu(_screenWidthPx, _screenHeightPx, _font, _spriteBatch);
            _gameHUD = new HUD(_mainGame, _spriteBatch, _font, _textureManager, _screenWidthPx, _screenHeightPx, _independentRenderer);
        }

        private Vector2 getScreenCenter()
        {
            return ConvertUnits.ToSimUnits(new Vector2(_screenWidthPx / 2f, _screenHeightPx / 2f));
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
            var restartGame = false;

            if (_gameState == GameState.Playing)
            {
                _mainGame.Update(gameTime, newKeyboardState);
                if(_mainGame.IsGameOver())
                {
                    _gameState = GameState.GameOver;
                }
            }
            else if(_gameState == GameState.GameOver)
            {
                
                if (newKeyboardState.IsKeyDown(Keys.Enter))
                {
                    restartGame = true;
                }
            }
            else if(_gameState == GameState.StartMenu)
            {
                if (newKeyboardState.IsKeyDown(Keys.Enter))
                {
                    restartGame = true;
                }
            }

            _gameHUD.Update(_gameState, newKeyboardState, _oldKeyboardState);

            // Let HUD update before restarting
            if(restartGame)
            {
                resetGame();
            }
            _oldKeyboardState = newKeyboardState;      

            base.Update(gameTime);
        }

        private void resetGame()
        {
            _mainGame.RestartGame();
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
                _startMenu.Draw(_gameHUD.GetHighScores());
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
