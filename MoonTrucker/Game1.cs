using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameContent gameContent;

        private Car car;
        private int screenWidth = 0;
        private int screenHeight = 0;
        private KeyboardState oldKeyboardState;

        public Game1()
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
            if (screenWidth >= 502)
            {
                screenWidth = 502;
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
            car = new Car(new TruckSprite(gameContent), carX, carY, screenWidth, screenHeight, spriteBatch);  // create the game paddle


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Up))
            {
                car.MoveUp();
            }
            if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                car.MoveDown();
            }

            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            car.Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
