using System;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonTrucker
{
    public class GeneratedCity
    {
        private GameContent _gameContent;
        private SpriteBatch _spriteBatch;
        private float _screenWidth;
        private float _screenHeight;
        private World _world;

        public GeneratedCity(GameContent gameContent, SpriteBatch spriteBatch, float screenWidth, float screenHeight, World world)
        {
            _gameContent = gameContent;
            _spriteBatch = spriteBatch;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _world = world;
        }

        public Wall[] GenerateSquareCity()
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

        

    }

    //TODO: Move this to it's own class
    public class SquareBody
    {

    }
}
