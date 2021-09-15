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

    public class StaticBodyFactory
    {
        private World _world;
        private SpriteBatch _spriteBatch;
        private TextureManager _textureManager;

        public StaticBodyFactory(World world, TextureManager manager, SpriteBatch batch)
        {
            _world = world;
            _textureManager = manager;
            _spriteBatch = batch;
        }

        public RectangleBody CreateRectangleBody(float width, float height, Vector2 origin)
        {
            return new RectangleBody(width, height, origin, _world, _textureManager, _spriteBatch);
        }
    }

    public class RectangleBody : IDrawable
    {
        private Body _wallBody;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public RectangleBody(float width, float height, Vector2 origin, World world, TextureManager manager, SpriteBatch batch)
        {
            _wallBody = BodyFactory.CreateRectangle(world, width, height, 1f, origin);
            _wallBody.BodyType = BodyType.Static;
            _wallBody.Restitution = 1f;
            _wallBody.Friction = 1f;
            _sprite = manager.TextureFromShape(_wallBody.FixtureList[0].Shape, Color.Aqua, Color.Wheat);
            _batch = batch;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_wallBody.Position),null, Color.White, _wallBody.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public static RectangleBody CreateRectangleBodyFromDisplayUnits(float width, float height, Vector2 origin, World world, TextureManager manager, SpriteBatch batch)
        {
            return new RectangleBody(ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), ConvertUnits.ToSimUnits(origin), world, manager, batch);
        }
    }
}
