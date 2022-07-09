using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

public class LinearParticleTrail
{
    private Vector2 _origin;
    private Vector2 _generationBounds;
    private Direction _direction;
    private int _lineCoordinate;
    private ParticleTrail _particleTrail;
    private int _size = 5;
    private float _startCoord;
    private float _endCoord;
    private Color _color;
    private Vector2 _currentPosition;
    private double _lastParticleTimeSeconds = 0.0;
    /// <summary>
    /// Particles per second created.
    /// </summary>
    private static int _frequency = 10;
    private double _secondsBetweenUpdates = 1 / _frequency;

    /// <summary>
    /// Draws a horiz/vert line of particles in a region that decay in opacity. Regions are always rectangular. Give top-left corner of region as origin. Give length/width as bounds.
    /// </summary>
    /// <param name="origin">Coordinates of the top left corner of the generation region. (in screen coords)</param>
    /// <param name="generationBounds">Dimensions of the region the line will run across (in screen coords)</param>
    /// <param name="dir">Whether the line should run horiz/vert.</param>
    /// <param name="lineCoord">The constant value in the equation of this line. If this is 50 and horiz, then y coord will always be 50. (in screen coords)</param>
    /// <param name="color"> The color of the particle</param>
    /// <param name="shouldBuffer"> If true will add a buffer to each end of generation region to randomize pixel start and end points.</param>
    /// <param name="sb"> The sprite batch</param>
    /// <param name="texMan"> The texture manager </param>
    public LinearParticleTrail(Vector2 origin, Vector2 generationBounds, Direction dir, int lineCoord, Color color, bool shouldBuffer, SpriteBatch sb, TextureManager texMan)
    {
        _origin = origin;
        _generationBounds = generationBounds;
        _direction = dir;
        _lineCoordinate = lineCoord;
        _color = color;
        _particleTrail = new ParticleTrail(_size, sb, texMan);
        this._setStartEndCurrentCoords(shouldBuffer);
        this._createParticleAtCurrentPosition();
    }

    private void _setStartEndCurrentCoords(bool shouldBuffer)
    {
        var rand = new Random();
        switch (_direction)
        {
            case Direction.Right:
                _startCoord = _origin.X + (shouldBuffer ? (float)rand.Next(-20, -5) : 0.0f);
                _endCoord = _generationBounds.X + (shouldBuffer ? 50f : 0.0f);
                _currentPosition = new Vector2(_startCoord, _lineCoordinate);
                break;
            case Direction.Down:
                _startCoord = _origin.Y + (shouldBuffer ? (float)rand.Next(-20, -5) : 0.0f);
                _endCoord = _generationBounds.Y + (shouldBuffer ? 50f : 0.0f);
                _currentPosition = new Vector2(_lineCoordinate, _startCoord);
                break;
            case Direction.Left:
                _startCoord = _origin.X + _generationBounds.X + (shouldBuffer ? (float)rand.Next(5, 20) : 0.0f);
                _endCoord = _origin.X + (shouldBuffer ? -50f : 0.0f);
                _currentPosition = new Vector2(_startCoord, _lineCoordinate);
                break;
            case Direction.Up:
                _startCoord = _origin.Y + _generationBounds.Y + (shouldBuffer ? (float)rand.Next(5, 20) : 0.0f);
                _endCoord = _origin.Y + (shouldBuffer ? -50f : 0.0f);
                _currentPosition = new Vector2(_lineCoordinate, _startCoord);
                break;
            default:
                break;
        }
    }

    private void _createParticleAtCurrentPosition()
    {
        _particleTrail.CreateParticle(_currentPosition, _color);
    }

    public void Update(GameTime gameTime)
    {
        if (gameTime.TotalGameTime.TotalSeconds - _lastParticleTimeSeconds > _secondsBetweenUpdates)
        {
            _lastParticleTimeSeconds = gameTime.TotalGameTime.TotalSeconds;
            _updateCurrentPosition();
            _particleTrail.Update();
            if (!isLocationBeyondGenerationBounds(_currentPosition))
            {
                _createParticleAtCurrentPosition();
            }
        }
    }

    private void _updateCurrentPosition()
    {
        switch (_direction)
        {
            case Direction.Right:
                _currentPosition = new Vector2(_currentPosition.X + _size, _lineCoordinate);
                break;
            case Direction.Down:
                _currentPosition = new Vector2(_lineCoordinate, _currentPosition.Y + _size);
                break;
            case Direction.Left:
                _currentPosition = new Vector2(_currentPosition.X - _size, _lineCoordinate);
                break;
            case Direction.Up:
                _currentPosition = new Vector2(_lineCoordinate, _currentPosition.Y - _size);
                break;
        }
    }

    public bool IsDone()
    {
        if (_particleTrail.isDone())
        {
            return true;
        }
        var oldestParticle = _particleTrail.GetCoordinateOfOldestParticle();
        return isLocationBeyondGenerationBounds(oldestParticle);
    }

    private bool isLocationBeyondGenerationBounds(Vector2 location)
    {
        switch (_direction)
        {
            case Direction.Right:
                return location.X > (_origin.X + _generationBounds.X);
            case Direction.Down:
                return location.Y > (_origin.Y + _generationBounds.Y);
            case Direction.Left:
                return location.X < _origin.X;
            case Direction.Up:
                return location.Y < _origin.Y;
            default:
                return true;
        }
    }

    public void Draw()
    {
        _particleTrail.Draw();
    }
}
public enum Direction
{
    Up,
    Right,
    Down,
    Left
}