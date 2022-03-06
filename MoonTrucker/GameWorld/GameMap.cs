using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    // TODO: Move tile logic to own class
    public class GameMap : IDrawable
    {
        private char[][] _tileMap;
        private List<IDrawable> _mapProps;
        private float _tileWidth;
        private PropFactory _propFactory;
        private Finish _finish; // TODO: This can be null if the map doesn't have a finish. Handle that more gracefully
        private LevelConfig _level;
        private int _numberOfTargets = 0;
        private List<GameTarget> _targets;

        private float _mapHeight => _tileMap.Length * _tileWidth;
        private float _mapWidth => _tileMap.Select(mapRow => mapRow.Length).Max() * _tileWidth;

        public   GameMap(LevelConfig level, float tileWidth, PropFactory propFactory)
        {
            _level = level;
            _tileWidth = tileWidth;
            _propFactory = propFactory;
            _targets = new List<GameTarget>();
        }

        public void Load()
        {
            _tileMap = loadMapFromFile();
            _mapProps = parseMap();
            _mapProps.AddRange(createWalls());
        }

        public void Draw()
        {
            foreach (IDrawable prop in _mapProps)
            {
                prop.Draw();
            }
        }

        public bool IsPlayerInWinZone()
        {
            return _finish is null ? false: _finish.IsPlayerInFinishZone();
        }

        public void ActivateFinish()
        {
            if(_finish != null)
            {
                _finish.MakeActive();
            }
        }

        public Vector2 GetFinishPosition()
        {
            return _finish is null ? new Vector2(0,0) :  _finish.GetPosition();
        }

        public int GetNumberOfTargets()
        {
            return _numberOfTargets;
        }

        // TODO: This is a service. It needs to be in it's own class and injected in
        private char[][] loadMapFromFile(bool shouldUseVehicleTestbench = false)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = shouldUseVehicleTestbench ? "MoonTrucker.GameWorld.TestBench.txt" : _level.MapName;
            char[][] tileMap;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
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
                    if ((char)propMapValue == '\r')
                    {
                        continue;
                    }
                    var curCoordinate = new MapCoordinate(row, col);
                    if (propMapValue != TileType.Road && isTopLeftCorner(curCoordinate))
                    {
                        Vector2 propDim = getPropDimensionsInSim(curCoordinate);
                        Vector2 curPosInSim = getCoordInSim(curCoordinate);

                        var prop = CreatePropBodyForTile(propMapValue, propDim, PropFactory.GetOriginFromDimensions(propDim, curPosInSim));
                        if (prop != null) { props.Add(prop); }
                    }
                }
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
            return Vector2.Add(getCoordInSim(new MapCoordinate(1, 1)), new Vector2(_tileWidth / 2f, _tileWidth / 2f));
        }

        public void SubscribeToTargets(IObserver<GameTarget> observer)
        {
            foreach(GameTarget target in _targets)
            {
                target.Subscribe(observer);
            }
        }

        private IDrawable CreatePropBodyForTile(TileType tile, Vector2 propDim, Vector2 origin)
        {
            switch (tile)
            {
                case TileType.Building:
                    return _propFactory.CreateRectangleBody(propDim.X, propDim.Y, origin);
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
                    return null; // TODO: DON'T RETURN NULLL
            }
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

            return new Point(width, height);
        }

        private Vector2 getPropDimensionsInSim(MapCoordinate startingCoordinate)
        {
            var dim = getPropDimensions(startingCoordinate);
            var simDim = dim.ToVector2() * _tileWidth;
            return simDim;
        }

        private Vector2 getCoordInSim(MapCoordinate cord)
        {

            return cord.ToVector2() * _tileWidth;
        }
    }

    public enum TileType
    {
        Building = 'B',
        Road = '_',
        Hidden = 'H',
        RestrictedRoad = 'X', //like a road but it can't have targets spawned on it
        Target = 'T',
        Gate = 'G',
        Finish = 'F'

    }
}
