using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class MoonTruckerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameContent _gameContent;

        private Vehicle _truck;
        private int _screenWidth = 0;
        private int _screenHeight = 0;
        private KeyboardState _oldKeyboardState;

        public MoonTruckerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/GameAssets";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _gameContent = new GameContent(Content);
            _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //set game to 502x700 or screen max if smaller
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
            int carX = (_screenWidth - _gameContent.ImgViperCar.Width) / 2;
            //we'll center the paddle on the screen to start
            int carY = _screenHeight - 300;  //paddle will be 100 pixels from the bottom of the screen
            _truck = new Vehicle(new TruckSprite(_gameContent, _spriteBatch), carX, carY, _screenWidth, _screenHeight);  // create the game paddle
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Up))
            {
                _truck.MoveUp();
            }
            if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                _truck.MoveDown();
            }

            _oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _truck.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
