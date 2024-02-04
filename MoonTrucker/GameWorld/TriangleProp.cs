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
                    return TriangleShape.DownLeft;
                case TileType.TriangleUL:
                    return TriangleShape.UpLeft;
                case TileType.TriangleUR:
                    return TriangleShape.UpRight;
                default:
                    return TriangleShape.DownRight;

            }
        }

        private Vertices GetVerticesForTriangleShape(TriangleShape shape, float height = 1f)
        {
            switch (shape)
            {
                case TriangleShape.DownLeft:
                    return new Vertices
                    {
                        new Vector2(0, 0),
                        new Vector2(0, height),
                        new Vector2(height, height),
                    };
                case TriangleShape.DownRight:
                    return new Vertices
                    {
                        new Vector2(height, 0),
                        new Vector2(0, height),
                        new Vector2(height, height),
                    };
                case TriangleShape.UpRight:
                    return new Vertices
                    {
                        new Vector2(0, 0),
                        new Vector2(height, 0),
                        new Vector2(height, height),
                    };
                case TriangleShape.UpLeft:
                default:
                    return new Vertices
                    {
                        new Vector2(height, 0),
                        new Vector2(0, height),
                        new Vector2(0, 0),
                    };
            }
        }


        public Body Body;
        private Texture2D _sprite;
        private SpriteBatch _batch;
        private Color _maksColor = Color.White;
        // TODO: Abstract away the parameters wolrd, manager, batch
        public TriangleProp(float height, Vector2 leftCorner, World world, TextureManager manager, SpriteBatch batch, TriangleShape shape, Color color, bool isSensor = false)
        {
            Vertices vertices = GetVerticesForTriangleShape(shape, height);
            Body = BodyFactory.CreatePolygon(world, vertices, 1f, leftCorner);
            Body.BodyType = BodyType.Static;
            //Body.Restitution = 0f;
            Body.Friction = .5f;
            Body.IsSensor = false;
            _sprite = manager.TextureFromShape(Body.FixtureList[0].Shape, color, Color.Black);
            _batch = batch;

            Body.OnCollision = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (isSensor)
                {
                    _maksColor = new Color(Color.Black, 0.2f);
                }
            };

            Body.OnSeparation = (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (isSensor)
                {
                    _maksColor = Color.White;
                }
            };
        }
        public void Draw()
        {
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(Body.Position), null, _maksColor, Body.Rotation, new Vector2(0, 0), 1f, SpriteEffects.None, .5f);
        }
    }
}
