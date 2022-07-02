using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;
using MoonTrucker.GameWorld;

public class ParticleGenerator : MoonTrucker.GameWorld.IDrawable
{
    private readonly int PARTICLE_LIMIT = 15;
    private Vector2 _origin;
    private Vector2 _end;
    private Direction _direction;
    private Color _color;
    private double _minTimeBetweenParticles = 0.15;
    private SpriteBatch _spriteBatch;
    private TextureManager _textureManager;
    private List<LinearParticleTrail> _racingParticles;
    /// <summary>
    /// The constant value of where particles should start generating. This is in the axis perpendicular to movement.
    /// </summary>
    private float generateStartCoord;

    /// <summary>
    /// The constant value of where particles should end. This is in the axis perpendicular to movement.
    /// </summary>
    private float generateEndCoord;


    /// <summary>
    /// Ctr
    /// ex.
    /// [>][ ][ ][ ][ ][B] - origin 0,0; end 4,0; dir Direction.Right; will send particles from left edge of origin to right edge of end
    /// 
    ///    ...
    /// ...[V] - origin 2,3; end 2,5; dir Direction.Down; will send particles from top edge of origin to bottom edge of end
    ///    [ ]
    ///    [ ]
    ///    [B]
    /// 
    /// </summary>
    /// <param name="origin">Coord of upper left corner of space</param>
    /// <param name="end">Coord of upper left cornner of end space. Will draw particles on end space.</param>
    /// <param name="singleGenDir">Required param only if origin and end are the same space. Otherwise ignored. Used to make a single space particle gen in the dir you want.</param>
    /// <param name="sb"></param>
    /// <param name="texMan"></param>
    public ParticleGenerator(Vector2 origin, Vector2 end, Direction? singleGenDir, SpriteBatch sb, TextureManager texMan)
    {
        _origin = origin;
        _end = end;
        this.parseCoordsSetDirection(singleGenDir);
        _color = this.getColorForDirection(_direction);
        _spriteBatch = sb;
        _textureManager = texMan;
        _racingParticles = new List<LinearParticleTrail>();
    }

    /// <summary>
    ///  Validate coordinates and sets direction
    /// </summary>
    /// <param name="singleGenDir"></param>
    /// <exception cref="ArgumentException"></exception>
    private void parseCoordsSetDirection(Direction? singleGenDir)
    {
        if (_origin.X == _end.X)
        {
            //Same point
            if (_origin.Y == _end.Y)
            {
                if (singleGenDir != null)
                {
                    _direction = (Direction)singleGenDir;
                    return;
                }
                throw new ArgumentException("If both origin and end are same space, dir must be provided.");
            }

            if (_origin.Y > _end.Y)
            {
                _direction = Direction.Up;
            }
            else
            {
                _direction = Direction.Down;
            }
        }
        else if (_origin.Y == _end.Y)
        {
            //Don't need to check if they're same point, we already know X doesn't match
            if (_origin.X > _end.X)
            {
                _direction = Direction.Left;
            }
            else
            {
                _direction = Direction.Right;
            }
        }
        throw new ArgumentException("Invalid coordinates. Must be in same row/column or same space (ie X == X || Y == Y or both). origin: (" + _origin.X + ", " + _origin.Y + "); end: (" + _end.X + ", " + _end.Y + ");");
    }

    private Color getColorForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Color.SkyBlue;
            case Direction.Down: return Color.LightYellow;
            case Direction.Left: return Color.LimeGreen;
            case Direction.Right: return Color.Purple;
            default: throw new ArgumentException("Invalid Direction");
        }
    }

    public void Draw()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// For a given map, starting position, and direction, iterates across map until it reaches a non road cell.
    /// </summary>
    /// <param name="map">The map to interate on.</param>
    /// <param name="dir">Direction the iteration should be done from start position</param>
    /// <param name="startPos">The start position of the search.</param>
    /// <returns>Map coordinate of last road tile that should be drawn on.</returns>
    internal static MapCoordinate FindGenEndPoint(char[][] map, Direction dir, MapCoordinate startPos)
    {
        switch (dir)
        {
            case Direction.Up:
                {
                    int lastValidIndex = startPos.Row;
                    for (int y = startPos.Row - 1; y >= 0; y--)
                    {
                        if (!ParticleGenerator.shouldGenerateOn(map[y][startPos.Column]))
                        {
                            return new MapCoordinate(lastValidIndex, startPos.Column);
                        }
                        lastValidIndex = y;
                    }
                    return new MapCoordinate(lastValidIndex, startPos.Column);
                }
            case Direction.Right:
                {
                    int lastValidIndex = startPos.Column;
                    for (int x = startPos.Column + 1; x < map[startPos.Row].Length; x++)
                    {
                        if (!ParticleGenerator.shouldGenerateOn(map[startPos.Row][x]))
                        {
                            return new MapCoordinate(startPos.Row, lastValidIndex);
                        }
                        lastValidIndex = x;
                    }
                    return new MapCoordinate(startPos.Row, lastValidIndex);
                }
            case Direction.Down:
                {
                    int lastValidIndex = startPos.Row;
                    for (int y = startPos.Row + 1; y < map.Length; y++)
                    {
                        if (!ParticleGenerator.shouldGenerateOn(map[y][startPos.Column]))
                        {
                            return new MapCoordinate(lastValidIndex, startPos.Column);
                        }
                        lastValidIndex = y;
                    }
                    return new MapCoordinate(lastValidIndex, startPos.Column);
                }
            case Direction.Left:
                {
                    int lastValidIndex = startPos.Column;
                    for (int x = startPos.Column - 1; x >= 0; x--)
                    {
                        if (!ParticleGenerator.shouldGenerateOn(map[startPos.Row][x]))
                        {
                            return new MapCoordinate(startPos.Row, lastValidIndex);
                        }
                        lastValidIndex = x;
                    }
                    return new MapCoordinate(startPos.Row, lastValidIndex);
                }
            default: return startPos;
        }
    }

    private static bool shouldGenerateOn(char c)
    {
        return (c == (char)TileType.Road)
        || (c == (char)TileType.RestrictedRoad)
        || (c == (char)TileType.Hidden);
    }
}