using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

public class FreeformParticleTrail
{

    private int _size;
    private Color _color;
    private SpriteBatch _spriteBatch;
    private TextureManager _textureManager;
    private ParticleTrail _particleTrail;
    public FreeformParticleTrail(int size, Color color, SpriteBatch sb, TextureManager texMan)
    {
        _size = size;
        _color = color;
        _spriteBatch = sb;
        _textureManager = texMan;
        _particleTrail = new ParticleTrail(_size, _spriteBatch, _textureManager);
    }

    public void CreateNewParticle(Vector2 position)
    {
        _particleTrail.CreateParticle(position, _color);
    }

    public void Update()
    {
        _particleTrail.Update();
    }

    public void Draw()
    {
        _particleTrail.Draw();
    }
}