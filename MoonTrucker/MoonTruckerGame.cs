using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class MoonTruckerGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameContent gameContent;

        private Vehicle truck;
        private int screenWidth = 0;
        private int screenHeight = 0;
        private KeyboardState oldKeyboardState;

        public MoonTruckerGame()
        {
            graphics = new GraphicsDeviceManager(this);
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            gameContent = new GameContent(Content);
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //set game to 502x700 or screen max if smaller
            if (screenWidth >= 1004)
            {
                screenWidth = 1004;
            }
            if (screenHeight >= 700)
            {
                screenHeight = 700;
            }
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            //create game objects
            int carX = (screenWidth - gameContent.ImgViperCar.Width) / 2;
            //we'll center the paddle on the screen to start
            int carY = screenHeight - 300;  //paddle will be 100 pixels from the bottom of the screen
            truck = new Vehicle(new TruckSprite(gameContent, spriteBatch), carX, carY, screenWidth, screenHeight);  // create the game paddle
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Up))
            {
                truck.MoveUp();
            }
            if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                truck.MoveDown();
            }

            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            truck.Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
