using System;using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Shared;

namespace MoonTrucker
{
    public class Wall
    {
        private Body _wallBody;
        private Texture2D _wallSprite;
        private Vector2 _wallOrigin;
        public Wall()
        {
        }

        public void createWall()
        {
            // // Convert screen center from pixels to meters
            // var screenCenter = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            // // Load Sprites
            // /* wall */
            // _wallSprite = Content.Load<Texture2D>("GroundSprite"); // 512px x 64px =>   8m x 1m
            // _wallOrigin = new Vector2(_wallSprite.Width / 2f, _wallSprite.Height / 2f);

            // Vector2 wallPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, 1.25f);

            // // Create the wall fixture
            // _wallBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, wallPosition);
            // _wallBody.BodyType = BodyType.Static;
            // _wallBody.Restitution = 0.3f;
            // _wallBody.Friction = 0.5f;
        }

        public void Draw()
        {
            //_spriteBatch.Draw(_wallSprite, ConvertUnits.ToDisplayUnits(_wallBody.Position), null, Color.White, 0f, _wallOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}
