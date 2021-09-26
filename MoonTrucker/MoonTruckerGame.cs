﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
using System.Collections.Generic;

namespace MoonTrucker
{
    public class MoonTruckerGame : Game
    {
        private bool _fullScreen = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private VehicleWithPhysics _vehicle;
        private GameMap _map;
        private int _screenWidthPx;
        private int _screenHeightPx;
        private KeyboardState _oldKeyboardState;
        private readonly World _world;
        private TextureManager _textureManager;
        private PropFactory _propFactory;
        private GameTarget _target;
        private Camera2D _camera;
        private ResolutionIndependentRenderer _independentRenderer;

        private const int _resolutionWidthPx = 1920;
        private const int _resolutionHeightPx = 1080;

        public MoonTruckerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            
            _world = new World(new Vector2(0, 0)); //Create a phyics world with no gravity

            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 14 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(14f);
        }

        protected override void Initialize()
        {
            setScreenDimensions();
            _independentRenderer = new ResolutionIndependentRenderer(this);
            _camera = new Camera2D(_independentRenderer);
            _camera.Zoom = 1f;
            _camera.Position = new Vector2(_screenWidthPx / 2f, _screenHeightPx / 2f);
            initializeResolutionIndependence(_screenWidthPx, _screenHeightPx);
            
            base.Initialize();
        }

        public void MoveTarget()
        {
            _target.Body.Body.Position = _map.GetRandomTargetLocation();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _textureManager = new TextureManager(Content, GraphicsDevice);
            _propFactory = new PropFactory(_world, _textureManager, _spriteBatch);
            
            //create game objects
            _vehicle = new VehicleWithPhysics(2f, 5f, getScreenCenter(), _world, _textureManager, _spriteBatch, GraphicsDevice);
            _map = generateMap();

            _target = new GameTarget(_vehicle.Width, Vector2.Add(getScreenCenter(), new Vector2(75, 0)), _propFactory, this);
        }

        // TODO: Move this somewhere else
        public GameMap generateMap()
        {
            var tileWidth = _vehicle.Height * 1.5f;
            return new GameMap(tileWidth, _propFactory, new Vector2(0, 0));
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

            _camera.RecalculateTransformationMatrices();
        }

        private void setScreenDimensions()
        {
            _screenWidthPx = _fullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : 1600;
            _screenHeightPx = _fullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height: 900; 

            _graphics.PreferredBackBufferWidth = _screenWidthPx;
            _graphics.PreferredBackBufferHeight = _screenHeightPx;
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            KeyboardState newKeyboardState = Keyboard.GetState();

            _vehicle.UpdateVehicle(newKeyboardState, gameTime);

            _oldKeyboardState = newKeyboardState;

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            _camera.Position = ConvertUnits.ToDisplayUnits(_vehicle.GetPosition());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _independentRenderer.BeginDraw();
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone,
                null, _camera.GetViewTransformationMatrix());

            _map.Draw();
            _target.Draw();
            _vehicle.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
