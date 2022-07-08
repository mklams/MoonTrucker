using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;
using MoonTrucker.GameWorld;

public class ParticleGenerator : MoonTrucker.GameWorld.IDrawable
{
    private readonly int PARTICLE_LIMIT = 15;
    private GameMap _map;
    private Vector2 _mapOrigin;
    private Vector2 _mapEnd;
    private Vector2 _simTopLeftOrigin;
    private Vector2 _simGenerationBounds;
    private Direction _direction;
    private Color _color;
    private double _minTimeBetweenRacingParticles = 0.15;
    private double _lastRacingParticleCreationTime = 0.0;
    private SpriteBatch _spriteBatch;
    private TextureManager _textureManager;
    private List<LinearParticleTrail> _racingParticles;
    private int _genMargin = 15;



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
    /// <param name="origin">Map coord of start gen space</param>
    /// <param name="end">Map coord of end gen space. Will draw particles on end space.</param>
    /// <param name="singleGenDir">Required param only if origin and end are the same space. Otherwise ignored. Used to make a single space particle gen in the dir you want.</param>
    /// <param name="sb"></param>
    /// <param name="texMan"></param>
    public ParticleGenerator(GameMap map, Vector2 origin, Vector2 end, Direction? singleGenDir, SpriteBatch sb, TextureManager texMan)
    {
        _map = map;
        _mapOrigin = origin;
        _mapEnd = end;
        this.parseCoordsSetDirection(singleGenDir);
        this.calculateSimOriginGenerationBounds();
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
        if (_mapOrigin.X == _mapEnd.X)
        {
            //Same point
            if (_mapOrigin.Y == _mapEnd.Y)
            {
                if (singleGenDir != null)
                {
                    _direction = (Direction)singleGenDir;
                    return;
                }
                throw new ArgumentException("If both origin and end are same space, dir must be provided.");
            }

            if (_mapOrigin.Y > _mapEnd.Y)
            {
                _direction = Direction.Up;
            }
            else
            {
                _direction = Direction.Down;
            }
        }
        else if (_mapOrigin.Y == _mapEnd.Y)
        {
            //Don't need to check if they're same point, we already know X doesn't match
            if (_mapOrigin.X > _mapEnd.X)
            {
                _direction = Direction.Left;
            }
            else
            {
                _direction = Direction.Right;
            }
        }
        throw new ArgumentException("Invalid coordinates. Must be in same row/column or same space (ie X == X || Y == Y or both). origin: (" + _mapOrigin.X + ", " + _mapOrigin.Y + "); end: (" + _mapEnd.X + ", " + _mapEnd.Y + ");");
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

    private void calculateSimOriginGenerationBounds()
    {
        if (_mapOrigin.Equals(_mapEnd))
        {
            _simTopLeftOrigin = _map.getCoordInSim(new MapCoordinate((int)_mapOrigin.X, (int)_mapOrigin.Y));
            _simGenerationBounds = _map.getCoordInSim(new MapCoordinate(1, 1));
        }
        switch (_direction)
        {
            case Direction.Right:
                _simTopLeftOrigin = _map.getCoordInSim(new MapCoordinate((int)_mapOrigin.X, (int)_mapOrigin.Y));
                _simGenerationBounds = _map.getCoordInSim(new MapCoordinate((int)(_mapEnd.X - _mapOrigin.X), 1));
                break;
            case Direction.Down:
                _simTopLeftOrigin = _map.getCoordInSim(new MapCoordinate((int)_mapOrigin.X, (int)_mapOrigin.Y));
                _simGenerationBounds = _map.getCoordInSim(new MapCoordinate(1, (int)(_mapEnd.Y - _mapOrigin.Y)));
                break;
            case Direction.Left:
                _simTopLeftOrigin = _map.getCoordInSim(new MapCoordinate((int)_mapEnd.X, (int)_mapEnd.Y));
                _simGenerationBounds = _map.getCoordInSim(new MapCoordinate((int)(_mapOrigin.X - _mapEnd.X), 1));
                break;
            case Direction.Up:
                _simTopLeftOrigin = _map.getCoordInSim(new MapCoordinate((int)_mapEnd.X, (int)_mapEnd.Y));
                _simGenerationBounds = _map.getCoordInSim(new MapCoordinate(1, (int)(_mapOrigin.Y - _mapEnd.Y)));
                break;
            default:
                break;
        }
    }

    public void Draw()
    {

    }

    public void Update(GameTime gameTime)
    {
        List<LinearParticleTrail> toRemove = new List<LinearParticleTrail>();
        _racingParticles.ForEach((lpt) =>
        {
            if (lpt.IsDone())
            {
                toRemove.Add(lpt);
            }
            else
            {
                lpt.Update(gameTime);
            }
        });
        toRemove.ForEach(lpt => _racingParticles.Remove(lpt));
        if (_racingParticles.Count < PARTICLE_LIMIT && gameTime.TotalGameTime.TotalSeconds - _lastRacingParticleCreationTime > _minTimeBetweenRacingParticles)
        {
            _lastRacingParticleCreationTime = gameTime.TotalGameTime.TotalSeconds;
            var rand = new Random();
            if (_direction == Direction.Right)
            {
                _racingParticles.Add(
                    new LinearParticleTrail(
                        _simTopLeftOrigin,
                        _simGenerationBounds,
                        _direction,
                        rand.Next((int)_simTopLeftOrigin.Y + _genMargin, (int)_simTopLeftOrigin.Y + (int)_simGenerationBounds.Y - _genMargin),
                        _color,
                        false,
                        _spriteBatch,
                        _textureManager
                    )
                );
            }
            else if (_direction == Direction.Down)
            {
                _racingParticles.Add(
                    new LinearParticleTrail(
                        _simTopLeftOrigin,
                        _simGenerationBounds,
                        _direction,
                        rand.Next((int)_simTopLeftOrigin.X + _genMargin, (int)_simTopLeftOrigin.X + (int)_simGenerationBounds.X - _genMargin),
                        _color,
                        false,
                        _spriteBatch,
                        _textureManager
                    )
                );
            }
            else if (_direction == Direction.Left)
            {
                _racingParticles.Add(
                    new LinearParticleTrail(
                        _simTopLeftOrigin,
                        _simGenerationBounds,
                        _direction,
                        rand.Next((int)_simTopLeftOrigin.Y + _genMargin, (int)_simTopLeftOrigin.Y + (int)_simGenerationBounds.Y - _genMargin),
                        _color,
                        false,
                        _spriteBatch,
                        _textureManager
                    )
                );
            }
            else if (_direction == Direction.Up)
            {
                _racingParticles.Add(
                    new LinearParticleTrail(
                        _simTopLeftOrigin,
                        _simGenerationBounds,
                        _direction,
                        rand.Next((int)_simTopLeftOrigin.X + _genMargin, (int)_simTopLeftOrigin.X + (int)_simGenerationBounds.X - _genMargin),
                        _color,
                        false,
                        _spriteBatch,
                        _textureManager
                    )
                );
            }
        }
        _racingParticles.ForEach(rp => rp.Update(gameTime));
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
        || (c == (char)TileType.Hidden);
    }
}