using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

public class ParticleTrail
{
    private List<Particle> _particles;
    private int _size;
    private SpriteBatch _spriteBatch;
    private TextureManager _textureManager;


    public ParticleTrail(int size, SpriteBatch spriteBatch, TextureManager texMan)
    {
        _particles = new List<Particle>();
        _size = size;
        _spriteBatch = spriteBatch;
        _textureManager = texMan;

    }

    public void CreateParticle(Vector2 pos, Color color)
    {
        _particles.Add(new Particle(_size, pos, color, _spriteBatch, _textureManager));
    }

    public void Update()
    {
        List<Particle> particlesToRemove = new List<Particle>();
        _particles.ForEach((p) =>
        {
            if (p.IsDone())
            {
                particlesToRemove.Add(p);
            }
            else
            {
                p.Update();
            }
        });
        particlesToRemove.ForEach(p => _particles.Remove(p));
    }

    public void Draw()
    {
        _particles.ForEach(p =>
        {
            if (!p.IsDone())
            {
                p.Draw();
            }
        });
    }

    public Vector2 GetCoordinateOfOldestParticle()
    {
        return _particles[0].Position;
    }
}