using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Shared;

namespace MoonTrucker
{
    public class MoonTruckerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameContent _gameContent;
        private VehicleWithPhysics _vehicle;
        private Tire _tire;
        private Wall[] _walls;
        private int _screenWidth;
        private int _screenHeight;
        private KeyboardState _oldKeyboardState;
        private readonly World _world;

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
            setScreenDimensions();

            //create game objects
            _vehicle = new VehicleWithPhysics(new CarSprite(_gameContent, _spriteBatch), _world, new Vector2(_screenWidth / 2,20), _screenWidth, _screenHeight);
            //_tire = new Tire(new TruckSprite(_gameContent, _spriteBatch), _world, new Vector2(_screenWidth / 2, 20), _screenWidth, _screenHeight);
            _walls = createWalls();
        }

        private void setScreenDimensions()
        {
            _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //set game to 1004x700 or screen max if smaller
            if (_screenWidth >= 1004)
            {
                _screenWidth = 1004;
            }
            if (_screenHeight >= 700)
            {
                _screenHeight = 700;
            }
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.ApplyChanges();
        }

        private Wall[] createWalls()
        {
            // Convert screen center from pixels to meters
            var screenCenter = new Vector2(_screenWidth / 2f, _screenHeight / 2f);

            return new Wall[] {
            new Wall(new RectangleWall(_gameContent, _spriteBatch, Color.Aqua, 5f, _screenHeight), _world, new Vector2(0,_screenHeight/2),  _screenWidth, _screenHeight),
            new Wall(new RectangleWall(_gameContent, _spriteBatch, Color.Aqua, 5f, _screenHeight), _world, new Vector2(_screenWidth - 5f, _screenHeight / 2), _screenWidth, _screenHeight),
            new Wall(new RectangleWall(_gameContent, _spriteBatch, Color.Aqua, _screenWidth, 5f), _world, new Vector2(_screenWidth / 2.0f, 0),  _screenWidth, _screenHeight),
            new Wall(new RectangleWall(_gameContent, _spriteBatch, Color.Aqua, _screenWidth, 5f), _world, new Vector2(_screenWidth / 2.0f, _screenHeight -5f), _screenWidth, _screenHeight),
            new Wall(new RectangleWall(_gameContent, _spriteBatch, Color.Aqua, 100f, 100f), _world, screenCenter, _screenWidth, _screenHeight)
            };
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
            foreach (Wall wall in _walls)
            {
                wall.Draw();
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
