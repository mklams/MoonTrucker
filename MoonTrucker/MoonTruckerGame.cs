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
        private int _screenWidth = 1004;
        private int _screenHeight = 700;
        private KeyboardState _oldKeyboardState;

        public MoonTruckerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/GameAssets";
            IsMouseVisible = true;
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
            _truck = new Vehicle(new TruckSprite(_gameContent, _spriteBatch), new Vector2(50,20), _screenWidth, _screenHeight);  // create the game paddle
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            KeyboardState newKeyboardState = Keyboard.GetState();

            _truck.UpdateVehicle(newKeyboardState, gameTime);

            _oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            _spriteBatch.Begin();
            _truck.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
