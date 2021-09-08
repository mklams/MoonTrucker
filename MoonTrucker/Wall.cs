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
        private Sprite _sprite;
        private Vector2 _wallOrigin;
        public Wall(Sprite wallSprite, World world, Vector2 position, float screenWidth, float screenHeight)
        {
            _sprite = wallSprite;
            
            // Convert screen center from pixels to meters
            var screenCenter = new Vector2(screenWidth / 2f, screenHeight / 2f);
            _wallOrigin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);

            Vector2 wallPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, 1.25f);

            // Create the wall fixture
            _wallBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, wallPosition);
            _wallBody.BodyType = BodyType.Static;
            _wallBody.Restitution = 0.3f;
            _wallBody.Friction = 0.5f;
        }

        public void Draw()
        {
            _sprite.Draw(ConvertUnits.ToDisplayUnits(_wallBody.Position), 0);
            //_spriteBatch.Draw(_wallSprite, ConvertUnits.ToDisplayUnits(_wallBody.Position), null, Color.White, 0f, _wallOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}
