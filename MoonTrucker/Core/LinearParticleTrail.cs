using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

public class LinearParticleTrail
{
    private Vector2 _screenDimensions;
    private Direction _direction;
    private int _lineCoordinate;
    private ParticleTrail _particleTrail;
    private int _size = 5;
    private int _startCoord;
    private int _endCoord;
    private Color _color;
    private Vector2 _currentPosition;
    private double _lastParticleTimeSeconds = 0.0;
    /// <summary>
    /// Particles per second created.
    /// </summary>
    private static int _frequency = 10;
    private double _secondsBetweenUpdates = 1 / _frequency;

    /// <summary>
    /// Draws a horiz/vert line of particles that decay in opacity. 
    /// </summary>
    /// <param name="screenDim">Dimensions of the screen the line will run across</param>
    /// <param name="dir">Whether the line should run horiz/vert.</param>
    /// <param name="lineCoord">The constant value in the equation of this line. If this is 50 and horiz, then y coord will always be 50</param>
    public LinearParticleTrail(Vector2 screenDim, Direction dir, int lineCoord, Color color, SpriteBatch sb, TextureManager texMan)
    {
        _screenDimensions = screenDim;
        _direction = dir;
        _lineCoordinate = lineCoord;
        _color = color;
        _particleTrail = new ParticleTrail(_size, sb, texMan);
        var rand = new Random();
        _startCoord = rand.Next(-20, -5);
        _endCoord = _direction == Direction.Horizontal ?
            (int)_screenDimensions.X + 50
            : (int)_screenDimensions.Y + 50;
        _currentPosition = _direction == Direction.Horizontal ?
            new Vector2(_startCoord, _lineCoordinate)
            : new Vector2(_lineCoordinate, _startCoord);
        this._createParticleAtCurrentPosition();
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
            _currentPosition = _direction == Direction.Horizontal ?
                new Vector2(_currentPosition.X + _size, _lineCoordinate)
                : new Vector2(_lineCoordinate, _currentPosition.Y + _size);
            _particleTrail.Update();
            _createParticleAtCurrentPosition();
        }
    }

    public bool IsDone()
    {
        var oldestParticle = _particleTrail.GetCoordinateOfOldestParticle();
        return _direction == Direction.Horizontal ?
            oldestParticle.X > _screenDimensions.X
            : oldestParticle.Y > _screenDimensions.Y;
    }

    public void Draw()
    {
        _particleTrail.Draw();
    }
}
public enum Direction
{
    Horizontal,
    Vertical
}