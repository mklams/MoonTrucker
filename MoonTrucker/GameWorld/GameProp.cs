using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Shared;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Handlers;
using MoonTrucker.Core;

namespace MoonTrucker.GameWorld
{

    public class PropFactory
    {
        private World _world;
        private SpriteBatch _spriteBatch;
        private TextureManager _textureManager;

        public PropFactory(World world, TextureManager manager, SpriteBatch batch)
        {
            _world = world;
            _textureManager = manager;
            _spriteBatch = batch;
        }

        public RectangleProp CreateRectangleBody(float width, float height, Vector2 origin)
        {
            return new RectangleProp(width, height, origin, _world, _textureManager, _spriteBatch);
        }

        public RectangleProp CreateRectangleSensor(float width, float height, Vector2 origin)
        {
            return new RectangleProp(width, height, origin, _world, _textureManager, _spriteBatch, true);
        }

        public CircleProp CreateCircleSensor(float radius, Vector2 origin, OnCollisionHandler onCollisionHandler = null, OnSeparationHandler onSeparationHandler = null)
        {
            var prop = new CircleProp(radius, origin, _world, _textureManager, _spriteBatch, true);

            // TODO: Violates demeters law
            prop.Body.OnCollision = onCollisionHandler;
            prop.Body.OnSeparation = onSeparationHandler;

            return prop;
        }

        public static Vector2 GetOriginFromDimensions(Vector2 dim, Vector2 topLeftCorner)
        {
            return new Vector2(topLeftCorner.X + dim.X / 2f, topLeftCorner.Y + dim.Y / 2f);
        }
    }

    public class RectangleProp : IDrawable
    {
        public Body Body;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        private Color _color = Color.White;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public RectangleProp(float width, float height, Vector2 origin, World world, TextureManager manager, SpriteBatch batch, bool isSensor = false)
        {
            Body = BodyFactory.CreateRectangle(world, width, height, 1f, origin);
            Body.BodyType = BodyType.Static;
            Body.Restitution = 0.1f;//your buildings were bouncy with Restitution=1.
            Body.Friction = 1f;
            Body.IsSensor = isSensor;
            _sprite = manager.TextureFromShape(Body.FixtureList[0].Shape, Color.Aqua, Color.Aquamarine);
            _batch = batch;

            Body.OnCollision = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (isSensor)
                {
                    _color = Color.Tomato;
                }
            };

            Body.OnSeparation = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (isSensor)
                {
                    _color = Color.White;
                }
            };
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(Body.Position), null, _color, Body.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }

    public class CircleProp : IDrawable
    {
        public readonly Body Body;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public CircleProp(float radius, Vector2 origin, World world, TextureManager manager, SpriteBatch batch, bool isSensor = false)
        {
            Body = BodyFactory.CreateCircle(world, radius, 1f, origin);
            Body.BodyType = BodyType.Static;
            Body.Restitution = 1f;
            Body.Friction = 1f;
            Body.IsSensor = isSensor;
            _sprite = manager.TextureFromShape(Body.FixtureList[0].Shape, Color.WhiteSmoke, Color.White);
            _batch = batch;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(Body.Position), null, Color.White, Body.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
