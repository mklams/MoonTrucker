using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Shared;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

namespace MoonTrucker.GameWorld
{
    public enum TriangleShape
    {
        DownLeft,
        DownRight,
        UpRight,
        UpLeft
    }

    

    public class TriangleProp : IDrawable
    {
        public static TriangleShape GetTrianglePropFromTile(TileType tile)
        {
            switch (tile)
            {
                case TileType.TriangleDR:
                    return TriangleShape.DownRight;
                case TileType.TriangleDL:
                    return TriangleShape.UpLeft;
                case TileType.TriangleUL:
                    return TriangleShape.UpLeft;
                case TileType.TriangleUR:
                    return TriangleShape.UpRight;
                default:
                    return TriangleShape.DownRight;

            }
        }

        public static float GetRotationsForTriangleShape(TriangleShape shape, float height = 1)
        {
            switch (shape)
            {
                case TriangleShape.DownLeft:
                    return 0;
                case TriangleShape.DownRight:
                    return 1.5708f;
                case TriangleShape.UpRight:
                    return 180;
                case TriangleShape.UpLeft:
                    return 270;
                default:
                    return 0;
            }
        }


        public Body Body;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        private Color _color = Color.White;
        private Vector2 _leftCorner;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public TriangleProp(float height, Vector2 leftCorner, World world, TextureManager manager, SpriteBatch batch, TriangleShape shape, bool isSensor = false)
        {
            _leftCorner = leftCorner;
            Vertices vertices = new Vertices
                {
                    new Vector2(0, 0),
                    new Vector2(height, height),
                    new Vector2(0, height)

                };
            float rotation = GetRotationsForTriangleShape(shape);

            Body = BodyFactory.CreatePolygon(world, vertices, 1f, _leftCorner, rotation);
            Body.BodyType = BodyType.Static;
            //Body.Restitution = 0f;
            Body.Friction = .5f;
            Body.IsSensor = false;
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
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(Body.Position), null, _color, Body.Rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
        }
    }
}
