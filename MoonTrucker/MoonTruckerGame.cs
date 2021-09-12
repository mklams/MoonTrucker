using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Shared;
using System.Collections.Generic;

namespace MoonTrucker
{
    public class MoonTruckerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameContent _gameContent;
        private VehicleWithPhysics _vehicle;
        private List<RectangleBody> _walls;
        private int _screenWidth;
        private int _screenHeight;
        private KeyboardState _oldKeyboardState;
        private readonly World _world;
        private TextureManager _textureManager;

        public MoonTruckerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/GameAssets";
            IsMouseVisible = true;

            _world = new World(new Vector2(0, 0)); //Create a phyics world with no gravity
            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            // TODO: Figure out if this is the convertion rate we want
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameContent = new GameContent(Content, GraphicsDevice);
            _textureManager = new TextureManager(Content, GraphicsDevice);
            setScreenDimensions();
            var screenCenter = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            //create game objects
            var vehicleSprite = new RectangleSprite(_gameContent, _spriteBatch, Color.AliceBlue, 20, 40);
            //_vehicle = new VehicleWithPhysics(vehicleSprite, _world, new Vector2(_screenWidth / 2,20), _screenWidth, _screenHeight);
            _vehicle = new VehicleWithPhysics(_world, _textureManager, _spriteBatch);
            var cityGenerator = new GeneratedCity(_gameContent, _spriteBatch, _screenWidth, _screenHeight, _world, _vehicle, _textureManager);
            _walls = cityGenerator.GenerateSquareCity();
        }

        private void setScreenDimensions()
        {
            _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //set game to 1004x700 or screen max if smaller
            if (_screenWidth >= 1500)
            {
                _screenWidth = 1500;
            }
            if (_screenHeight >= 1000)
            {
                _screenHeight = 1000;
            }
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _vehicle.Draw();
            foreach (RectangleBody wall in _walls)
            {
                wall.Draw();
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
