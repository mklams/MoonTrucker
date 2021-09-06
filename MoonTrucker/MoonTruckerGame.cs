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

        private Car _truck;
        private int _screenWidth = 1004;
        private int _screenHeight = 700;
        private KeyboardState _oldKeyboardState;

        private readonly World _world;
        private Body _carBody;
        private Vector2 _carOrigin;
        private Body _circleBody;
        private Texture2D _circleSprite;
        private Vector2 _circleOrigin;

        private Body _wallBody;
        private Texture2D _wallSprite;
        private Vector2 _wallOrigin;
        

        public MoonTruckerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/GameAssets";
            IsMouseVisible = false;

            //_world = new World(new Vector2(0, 0)); //Create a phyics world with no gravity
            _world = new World(new Vector2(0, 9.82f));
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameContent = new GameContent(Content);
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

            //create game objects
            _truck = new Car(new CarSprite(_gameContent, _spriteBatch), new Vector2(50,20), _screenWidth, _screenHeight);  // create the game paddle

            createPhysicsBodies();
        }

        private void createPhysicsBodies()
        {
            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            // TODO: Figure out if this is the convertion rate we want
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            // Convert screen center from pixels to meters
            var screenCenter = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            // Load Sprites
            /* wall */
            _wallSprite = Content.Load<Texture2D>("GroundSprite"); // 512px x 64px =>   8m x 1m
            _wallOrigin = new Vector2(_wallSprite.Width / 2f, _wallSprite.Height / 2f);

            Vector2 wallPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, 1.25f);

            // Create the wall fixture
            _wallBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, wallPosition);
            _wallBody.BodyType = BodyType.Static;
            _wallBody.Restitution = 0.3f;
            _wallBody.Friction = 0.5f;

            /* circle */
            _circleSprite = Content.Load<Texture2D>("CircleSprite"); //  96px x 96px => 1.5m x 1.5m
            _circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);
            
            Vector2 circlePosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, -1.5f);
            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(96 / 2f), 1f, circlePosition, BodyType.Dynamic);
            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            /* car */
            _carBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, wallPosition);
            _carBody.BodyType = BodyType.Dynamic;
            _carBody.Restitution = 0.3f;
            _carBody.Friction = 0.5f; // TODO: Does friction do anything in a no gravity world?

            // _carBody.Position = ConvertUnits.ToSimUnits(new Vector2(50,50)); // TODO: Do this before creating the sprite object so that the position can for the sprite can be pulled from the body position
            // _carBody.CreateFixture(carShape);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            KeyboardState newKeyboardState = Keyboard.GetState();

            _truck.UpdateVehicle(newKeyboardState, gameTime);

            handlePhysicsEngineKeyboardInput();
            _oldKeyboardState = newKeyboardState;

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void handlePhysicsEngineKeyboardInput()
        {
            KeyboardState state = Keyboard.GetState();
             // We make it possible to rotate the circle body
            if (state.IsKeyDown(Keys.A))
                _circleBody.ApplyTorque(-10);

            if (state.IsKeyDown(Keys.D))
                _circleBody.ApplyTorque(10);

            if (state.IsKeyDown(Keys.Space) && _oldKeyboardState.IsKeyUp(Keys.Space))
                _circleBody.ApplyLinearImpulse(new Vector2(0, -10));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            _spriteBatch.Begin();
            _truck.Draw();

             _spriteBatch.Draw(_circleSprite, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, _circleBody.Rotation, _circleOrigin, 1f, SpriteEffects.None, 0f);
             _spriteBatch.Draw(_wallSprite, ConvertUnits.ToDisplayUnits(_wallBody.Position), null, Color.White, 0f, _wallOrigin, 1f, SpriteEffects.None, 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
