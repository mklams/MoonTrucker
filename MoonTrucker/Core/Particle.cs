
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;
/// <summary>
/// A square particle
/// </summary>
public class Particle
{
    /// <summary>
    /// The height/width of the particle in pixels
    /// </summary>
    private int _size { get; set; }
    /// <summary>
    /// Value from 0-100
    /// </summary>
    private float _opacity { get; set; }
    public Vector2 Position { get; private set; }
    private Color _color { get; set; }
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    public Particle(int size, Vector2 pos, Color color, SpriteBatch spriteBatch, TextureManager texMan)
    {
        _pixel = texMan.GetTexture("pixel");
        _spriteBatch = spriteBatch;
        _size = size;
        _opacity = 100f;
        Position = pos;
        _color = color;
    }

    public void Draw()
    {
        _spriteBatch.Draw(_pixel, new Rectangle((int)Position.X, (int)Position.Y, _size, _size), _color * (_opacity / 300f));
    }

    public bool IsDone()
    {
        return _opacity <= 0;
    }

    public void Update()
    {
        _opacity -= 1;
    }
}