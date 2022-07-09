using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

namespace MoonTrucker.GameWorld
{
    // TODO: Move tile logic to own class
    public class GameMap : IDrawable
    {
        private static HashSet<char> _generatorTiles = new HashSet<char>() { (char)TileType.GenUp, (char)TileType.GenDown, (char)TileType.GenLeft, (char)TileType.GenRight };
        private char[][] _tileMap;
        private List<IDrawable> _mapProps;
        private float _tileWidth;
        private PropFactory _propFactory;
        private Finish _finish; // TODO: This can be null if the map doesn't have a finish. Handle that more gracefully
        private LevelConfig _level;
        private int _numberOfTargets = 0;
        private List<GameTarget> _targets;
        private List<ParticleGenerator> _particleGens;
        private MapCoordinate _startLocation;
        private SpriteBatch _spriteBatch;
        private TextureManager _textureManager;

        private float _mapHeight => _tileMap.Length * _tileWidth;
        private float _mapWidth => _tileMap.Select(mapRow => mapRow.Length).Max() * _tileWidth;

        public GameMap(LevelConfig level, float tileWidth, PropFactory propFactory)
        {
            _level = level;
            _tileWidth = tileWidth;
            _propFactory = propFactory;
            _targets = new List<GameTarget>();
            _particleGens = new List<ParticleGenerator>();
        }

        public void Load()
        {
            _tileMap = loadMapFromFile();
            _mapProps = parseMap();
            _mapProps.AddRange(createWalls());
        }

        public void InitializeGraphics(SpriteBatch sb, TextureManager texMan)
        {
            _spriteBatch = sb;
            _textureManager = texMan;
        }

        public void Draw()
        {
            foreach (IDrawable prop in _mapProps)
            {
                prop.Draw();
            }
        }

        public void Update(GameTime gameTime){
            _particleGens.ForEach(pg => pg.Update(gameTime));
        }

        public bool IsPlayerInWinZone()
        {
            return HasFinish() ?  _finish.IsPlayerInFinishZone(): false;
        }

        public void ActivateFinish()
        {
            if (HasFinish())
            {
                _finish.MakeActive();
            }
        }

        public bool HasFinish()
        {
            return _finish != null;
        }

        public Vector2 GetFinishPosition()
        {
            return HasFinish() ? _finish.GetPosition(): new Vector2(0, 0);
        }

        public int GetNumberOfTargets()
        {
            return _numberOfTargets;
        }

        // TODO: This is a service. It needs to be in it's own class and injected in
        private char[][] loadMapFromFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            char[][] tileMap;

            using (Stream stream = assembly.GetManifestResourceStream(_level.MapName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string tileString = sanitizeMapString(reader.ReadToEnd());
                tileMap = tileString
                                 .Split("\n")
                                 .Select(line => line.ToCharArray())
                                 .ToArray();
            }

            return tileMap;
        }

        /// <summary>
        /// Removes all chars that aren't present in TileType enum or are \n delimiters.
        /// </summary>
        /// <param name="tileString"></param>
        /// <returns>The sanitized string.</returns>
        private string sanitizeMapString(string tileString)
        {
            return new string(tileString.Where(c => (c == '\n') || Enum.IsDefined(typeof(TileType), (int)c)).Select(c => c).ToArray());
        }

        private List<IDrawable> parseMap()
        {
            var props = new List<IDrawable>();
            for (int row = 0; row < _tileMap.Length; row++)
            {
                for (int col = 0; col < _tileMap[row].Length; col++)
                {
                    var propMapValue = (TileType)_tileMap[row][col];
                    var curCoordinate = new MapCoordinate(row, col);

                    if (propMapValue == TileType.Start)
                    {
                        _startLocation = curCoordinate;
                        continue;
                    }

                    if ((char)propMapValue == '\r' || propMapValue == TileType.Road)
                    {
                        continue;
                    }


                    if (!isFirstInBlock(curCoordinate)) { continue; }

                    Vector2 propDim = getPropDimensionsInSim(curCoordinate);
                    Vector2 curPosInSim = getCoordInSim(curCoordinate);
                    IDrawable prop;
                    if (_generatorTiles.Contains((char)propMapValue))
                    {
                        prop = CreatePropGraphicForTile(propMapValue, curCoordinate);
                    }
                    else
                    {
                        prop = CreatePropBodyForTile(propMapValue, propDim, PropFactory.GetOriginFromDimensions(propDim, curPosInSim), curPosInSim);
                    }
                    if (prop != null) { props.Add(prop); }
                }
            }

            if (_targets.Count < 1)
            {
                // TODO: Handle this more gracefully
                throw new Exception("Invalid game map. Map must have at least 1 target (T)");
            }

            return props;
        }

        private List<IDrawable> createWalls()
        {
            const float wallWidth = 2f;
            List<IDrawable> walls = new List<IDrawable>();
            var topWall = _propFactory.CreateRectangleBody(_mapWidth, wallWidth, new Vector2(_mapWidth / 2f, wallWidth / 2f));
            walls.Add(topWall);

            var leftWall = _propFactory.CreateRectangleBody(wallWidth, _mapHeight, new Vector2(wallWidth / 2f, _mapHeight / 2f));
            walls.Add(leftWall);

            var rightWall = _propFactory.CreateRectangleBody(wallWidth, _mapHeight, new Vector2(_mapWidth - (wallWidth / 2f), _mapHeight / 2f));
            walls.Add(rightWall);

            var bottomWall = _propFactory.CreateRectangleBody(_mapWidth, wallWidth, new Vector2(_mapWidth / 2f, _mapHeight - (wallWidth / 2f)));
            walls.Add(bottomWall);

            return walls;
        }

        /// <summary>
        /// Returns a radnom location to place a target. 
        /// </summary>
        /// <returns>The center position of the tile to place the target on</returns>
        public Vector2 GetRandomTargetLocation()
        {
            Vector2 location = new Vector2(0, 0);
            bool foundLocation = false;
            Random randomGen = new Random();
            while (!foundLocation)
            {
                int randomRow = randomGen.Next(_tileMap.Length);
                int randomCol = randomGen.Next(_tileMap[randomRow].Length);

                var tile = (TileType)_tileMap[randomRow][randomCol];

                if (tile == TileType.Road)
                {
                    foundLocation = true;
                    location = getCoordInSim(new MapCoordinate(randomRow, randomCol));
                }
            }

            return Vector2.Add(location, new Vector2(_tileWidth / 2f, _tileWidth / 2f));
        }

        public Vector2 GetStartPosition()
        {
            if (_startLocation is null)
            {
                return Vector2.Add(getCoordInSim(new MapCoordinate(1, 1)), new Vector2(_tileWidth / 2f, _tileWidth / 2f));
            }
            return getCoordInSim(_startLocation);
        }

        public void SubscribeToTargets(IObserver<GameTarget> observer)
        {
            foreach (GameTarget target in _targets)
            {
                target.Subscribe(observer);
            }
        }

        private IDrawable CreatePropBodyForTile(TileType tile, Vector2 propDim, Vector2 origin, Vector2 leftCorner)
        {
            switch (tile)
            {
                case TileType.Building:
                    return _propFactory.CreateRectangleBody(propDim.X, propDim.Y, origin);
                case TileType.BuildingAngled:
                case TileType.TriangleDL:
                case TileType.TriangleDR:
                case TileType.TriangleUR:
                case TileType.TriangleUL:
                    TriangleShape shape = TriangleProp.GetTrianglePropFromTile(tile);
                    return _propFactory.CreateTriangleBody(propDim.X, leftCorner, shape);
                case TileType.Hidden:
                    return _propFactory.CreateRectangleSensor(propDim.X, propDim.Y, origin);
                case TileType.Finish:
                    _finish = new Finish(propDim.X / 2f, origin, _propFactory);
                    return _finish;
                case TileType.Target:
                    var target = new GameTarget(_tileWidth / 4f, origin, _propFactory);
                    _targets.Add(target);
                    _numberOfTargets++;
                    return target;
                default:
                    return null; // TODO: DON'T RETURN NULL
            }
        }

        private IDrawable CreatePropGraphicForTile(TileType tile, MapCoordinate currCoord)
        {
            Vector2 originSimCoord = getCoordInSim(currCoord);
            ParticleGenerator partGen;
            Vector2 end;
            switch (tile)
            {
                case TileType.GenUp:
                    end = getCoordInSim(ParticleGenerator.FindGenEndPoint(_tileMap, Direction.Up, currCoord));
                    partGen = new ParticleGenerator(this, originSimCoord, end, Direction.Up, _spriteBatch, _textureManager);
                    _particleGens.Add(partGen);
                    break;
                case TileType.GenRight:
                    end = getCoordInSim(ParticleGenerator.FindGenEndPoint(_tileMap, Direction.Right, currCoord));
                    partGen = new ParticleGenerator(this, originSimCoord, end, Direction.Right, _spriteBatch, _textureManager);
                    _particleGens.Add(partGen);
                    break;
                case TileType.GenDown:
                    end = getCoordInSim(ParticleGenerator.FindGenEndPoint(_tileMap, Direction.Down, currCoord));
                    partGen = new ParticleGenerator(this, originSimCoord, end, Direction.Down, _spriteBatch, _textureManager);
                    _particleGens.Add(partGen);
                    break;
                case TileType.GenLeft:
                    end = getCoordInSim(ParticleGenerator.FindGenEndPoint(_tileMap, Direction.Left, currCoord));
                    partGen = new ParticleGenerator(this, originSimCoord, end, Direction.Left, _spriteBatch, _textureManager);
                    _particleGens.Add(partGen);
                    break;
                default: return null;
            }

            return partGen;
        }

        private TileType getTileAtCoordinate(MapCoordinate coordinate)
        {
            return (TileType)_tileMap[coordinate.Row][coordinate.Column];
        }

        private bool isFirstInBlock(MapCoordinate coordinate)
        {
            var propMapValue = getTileAtCoordinate(coordinate);
            if (propMapValue != TileType.Building && propMapValue != TileType.Hidden)
            {
                // TODO: Elevate this check. For now only do rectangle parsing with Buildings
                return true;
            }

            return !doesTileAtCoordinateMatchValue((char)propMapValue, new MapCoordinate(coordinate.Row, coordinate.Column - 1));
        }

        private bool isTopLeftCorner(MapCoordinate coordinate)
        {
            char propMapValue = _tileMap[coordinate.Row][coordinate.Column];

            return !doesTileAtCoordinateMatchValue(propMapValue, new MapCoordinate(coordinate.Row - 1, coordinate.Column))
                    && !doesTileAtCoordinateMatchValue(propMapValue, new MapCoordinate(coordinate.Row, coordinate.Column - 1));
        }

        private bool doesTileAtCoordinateMatchValue(char value, MapCoordinate coordinate)
        {
            if (coordinate.Row < 0 || coordinate.Column < 0)
            {
                return false;
            }

            return _tileMap[coordinate.Row][coordinate.Column] == value;
        }

        // Get dimensions in array index
        // TODO: Don't use point since this is a dimension
        private Point getPropDimensions(MapCoordinate startingCoordinate)
        {
            int width = 0;
            int height = 0;

            char propMapValue = _tileMap[startingCoordinate.Row][startingCoordinate.Column];
            // TODO: Use height or get rid of it
            for (int curRow = startingCoordinate.Row; curRow < _tileMap.Length; curRow++)
            {
                if (_tileMap[curRow][startingCoordinate.Column] != propMapValue)
                {
                    break;
                }
                height++;
            }

            for (int curCol = startingCoordinate.Column; curCol < _tileMap[startingCoordinate.Row].Length; curCol++)
            {
                if (_tileMap[startingCoordinate.Row][curCol] != propMapValue)
                {
                    break;
                }
                width++;
            }

            return new Point(width, 1);
            //return new Point(width, height);
        }

        private Vector2 getPropDimensionsInSim(MapCoordinate startingCoordinate)
        {
            var dim = getPropDimensions(startingCoordinate);
            var simDim = dim.ToVector2() * _tileWidth;
            return simDim;
        }

        public Vector2 getCoordInSim(MapCoordinate coord)
        {

            return coord.ToVector2() * _tileWidth;
        }
    }

    public enum TileType
    {
        Building = 'B',
        BuildingAngled = 'A',
        Road = '_',
        Hidden = 'H',
        PenaltyZone = 'X',
        Target = 'T',
        Gate = 'G',
        Finish = 'F',
        Start = 'S',
        GenUp = '^',
        GenRight = '>',
        GenDown = 'V',
        GenLeft = '<',
        TriangleDL = '1',
        TriangleDR = '2',
        TriangleUR = '3',
        TriangleUL = '4',
    }
}
