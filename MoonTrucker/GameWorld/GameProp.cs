using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
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
        private Color _objectColor;

        public PropFactory(World world, TextureManager manager, SpriteBatch batch)
        {
            _world = world;
            _textureManager = manager;
            _spriteBatch = batch;
            _objectColor = Color.Aqua;
        }

        public void SetObjectColor(Color color)
        {
            _objectColor = color;
        }

        public TriangleProp CreateTriangleBody(float height, Vector2 leftCorner, TriangleShape shape)
        {
            return new TriangleProp(height, leftCorner, _world, _textureManager, _spriteBatch, shape, _objectColor);
        }

        public RectangleProp CreateRectangleBody(float width, float height, Vector2 origin)
        {
            return new RectangleProp(width, height, origin, _world, _textureManager, _spriteBatch, _objectColor);
        }

        public RectangleProp CreateRectangleSensor(float width, float height, Vector2 origin)
        {
            return new RectangleProp(width, height, origin, _world, _textureManager, _spriteBatch, _objectColor, true);
        }

        public CircleProp CreateCircleSensor(float radius, Vector2 origin, Texture2D? texture, OnCollisionHandler onCollisionHandler = null, OnSeparationHandler onSeparationHandler = null)
        {
            var prop = new CircleProp(radius, origin, _world, _textureManager, _spriteBatch, texture, true);

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
        private Color _maskColor = Color.White;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public RectangleProp(float width, float height, Vector2 origin, World world, TextureManager manager, SpriteBatch batch, Color color, bool isSensor = false)
        {
            Body = BodyFactory.CreateRectangle(world, width, height, 1f, origin);
            Body.BodyType = BodyType.Static;
            Body.Restitution = 0f;//your buildings were bouncy with Restitution=1.
            Body.Friction = .5f;
            Body.IsSensor = isSensor;
            _sprite = manager.TextureFromShape(Body.FixtureList[0].Shape, color, color);
            _batch = batch;

            Body.OnCollision = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (isSensor)
                {
                    _maskColor = new Color(Color.Gray, 0.5f);
                }
            };

            Body.OnSeparation = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (isSensor)
                {
                    _maskColor = Color.White;
                }
            };
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(Body.Position), null, _maskColor, Body.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }

    public class CircleProp : IDrawable
    {
        public readonly Body Body;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        private Color _color = Color.White;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public CircleProp(float radius, Vector2 origin, World world, TextureManager manager, SpriteBatch batch, Texture2D? texture, bool isSensor = false)
        {
            Body = BodyFactory.CreateCircle(world, radius, 1f, origin);
            Body.BodyType = BodyType.Static;
            Body.Restitution = 1f;
            Body.Friction = 1f;
            Body.IsSensor = isSensor;
            _sprite = (texture != null) ? texture : manager.TextureFromShape(Body.FixtureList[0].Shape, Color.White, Color.White);
            _batch = batch;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void UpdateSprite(Texture2D texture)
        {
            _sprite = texture;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(Body.Position), null, _color, Body.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
