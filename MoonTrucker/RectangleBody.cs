using System;using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Shared;
using Genbox.VelcroPhysics.Collision.ContactSystem;

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

        public RectangleBody CreateRectangleSensor(float width, float height, Vector2 origin)
        {
            return new RectangleBody(width, height, origin, _world, _textureManager, _spriteBatch, true);
        }

        public CircleBody CreateCircleSensor(float radius, Vector2 origin)
        {
            return new CircleBody(radius, origin, _world, _textureManager, _spriteBatch, true);
        }
    }

    public class RectangleBody
    {
        private Body _wallBody;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        private Color _color = Color.White;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public RectangleBody(float width, float height, Vector2 origin, World world, TextureManager manager, SpriteBatch batch, bool isSensor = false)
        {
            _wallBody = BodyFactory.CreateRectangle(world, width, height, 1f, origin);
            _wallBody.BodyType = BodyType.Static;
            _wallBody.Restitution = 1f;
            _wallBody.Friction = 1f;
            _wallBody.IsSensor = isSensor;
            _sprite = manager.TextureFromShape(_wallBody.FixtureList[0].Shape, Color.Aqua, Color.Aquamarine);
            _batch = batch;

            _wallBody.OnCollision = (Fixture fixtureA, Fixture fixtureB, Contact contact) => {
                if (isSensor)
                {
                    _color = Color.Tomato;
                }
            };

            _wallBody.OnSeparation = (Fixture fixtureA, Fixture fixtureB, Contact contact) => {
                if (isSensor)
                {
                    _color = Color.White;
                }
            };
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_wallBody.Position),null, _color, _wallBody.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }

    public class CircleBody
    {
        private Body _body;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public CircleBody(float radius, Vector2 origin, World world, TextureManager manager, SpriteBatch batch, bool isSensor = false)
        {
            _body = BodyFactory.CreateCircle(world, radius, 1f, origin);
            _body.BodyType = BodyType.Static;
            _body.Restitution = 1f;
            _body.Friction = 1f;
            _body.IsSensor = isSensor;
            _sprite = manager.TextureFromShape(_body.FixtureList[0].Shape, Color.WhiteSmoke, Color.White);
            _batch = batch;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_body.Position), null, Color.White, _body.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
