using System.Collections.Generic;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;
using MoonTrucker.GameWorld;

public class GridOverlay : MoonTrucker.GameWorld.IDrawable
{
    private GameMap _gameMap;
    private SpriteBatch _spriteBatch;
    private TextureManager _textureManager;
    private List<Rectangle> _grid;
    private Color _color;
    private float _tileWidth;
    private float _infiniteIllusionBuffer = 6;

    private Texture2D _pixel;
    private Vector2 _mapSize;

    public GridOverlay(GameMap gm, Color color, SpriteBatch sb, TextureManager texMan)
    {
        _pixel = texMan.GetTexture("pixel");
        _color = color;
        _gameMap = gm;
        _tileWidth = _gameMap.GetTileWidth();
        _infiniteIllusionBuffer *= _tileWidth;
        _spriteBatch = sb;
        _textureManager = texMan;
        _grid = new List<Rectangle>();
        _mapSize = this.calculateMapSize();
        this.initialize();
    }

    public void Draw()
    {
        _grid.ForEach(r => _spriteBatch.Draw(_pixel, r, _color * 0.5f));
    }

    private void initialize()
    {
        for (float i = ConvertUnits.ToDisplayUnits(-_infiniteIllusionBuffer); i < ConvertUnits.ToDisplayUnits(_gameMap.Width + _infiniteIllusionBuffer); i += ConvertUnits.ToDisplayUnits(_tileWidth))
        {
            _grid.Add(new Rectangle((int)i, (int)-ConvertUnits.ToDisplayUnits(_gameMap.Height + _infiniteIllusionBuffer), 3, (int)(8 * ConvertUnits.ToDisplayUnits(_gameMap.Height))));
        }
        for (float j = ConvertUnits.ToDisplayUnits(-_infiniteIllusionBuffer); j < ConvertUnits.ToDisplayUnits(_gameMap.Height + _infiniteIllusionBuffer); j += ConvertUnits.ToDisplayUnits(_tileWidth))
        {
            _grid.Add(new Rectangle((int)-ConvertUnits.ToDisplayUnits(_gameMap.Width + _infiniteIllusionBuffer), (int)j, (int)(8 * ConvertUnits.ToDisplayUnits(_gameMap.Height + _infiniteIllusionBuffer)), 3));
        }
    }

    private Vector2 calculateMapSize()
    {
        return new Vector2(_gameMap.Width, _gameMap.Height);
    }
}