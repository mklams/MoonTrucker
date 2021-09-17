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
        private VehicleWithPhysics _vehicle;
        private List<IDrawable> _city;
        private int _screenWidth;
        private int _screenHeight;
        private KeyboardState _oldKeyboardState;
        private readonly World _world;
        private TextureManager _textureManager;
        private StaticBodyFactory _bodyFactory;
        private Camera2D _camera;
        private ResolutionIndependentRenderer _independentRenderer;

        public MoonTruckerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _independentRenderer = new ResolutionIndependentRenderer(this);
            _camera = new Camera2D(_independentRenderer);
            

            _world = new World(new Vector2(0, 0)); //Create a phyics world with no gravity
            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 14 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(14f);
        }

        protected override void Initialize()
        {
            setScreenDimensions();
            _camera.Zoom = 1f;
            _camera.Position = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            initializeResolutionIndependence(_screenWidth, _screenHeight);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _textureManager = new TextureManager(Content, GraphicsDevice);
            _bodyFactory = new StaticBodyFactory(_world, _textureManager, _spriteBatch);
            
            var screenCenterInSim = ConvertUnits.ToSimUnits(new Vector2(_screenWidth / 2f, _screenHeight / 2f));
            //create game objects
            _vehicle = new VehicleWithPhysics(2f, 5f, screenCenterInSim, _world, _textureManager, _spriteBatch, GraphicsDevice);
            var cityGenerator = new GeneratedCity(_bodyFactory, _screenWidth, _screenHeight, _vehicle);
            _city = cityGenerator.GenerateSquareCity();
        }

        private void initializeResolutionIndependence(int realScreenWidth, int realScreenHeight)
        {
            _independentRenderer.VirtualWidth = 1366;
            _independentRenderer.VirtualHeight = 768;
            _independentRenderer.ScreenWidth = realScreenWidth;
            _independentRenderer.ScreenHeight = realScreenHeight;
            _independentRenderer.Initialize();

            _camera.RecalculateTransformationMatrices();
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

            _camera.Position = ConvertUnits.ToDisplayUnits(_vehicle.GetPosition());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _independentRenderer.BeginDraw();
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone,
                null, _camera.GetViewTransformationMatrix());
            _vehicle.Draw();
            foreach (IDrawable body in _city)
            {
                body.Draw();
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
