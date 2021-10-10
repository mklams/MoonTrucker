using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    // TODO: Move tile logic to own class
    // TODO: Should this or the main game be observing GameTarget?

    public class GameMap : IDrawable, IObserver<GameTarget>
    {
        private char[][] _tileMap;
        private List<IDrawable> _mapProps;
        private float _tileWidt;
        private PropFactory _propFactory;
        private Vector2 _topLeftCorner;
        private IDisposable _cancellation;

        public GameMap(float tileWidth, PropFactory propFactory, Vector2 topLeftCorner)
        {
            _tileWidt = tileWidth;
            _propFactory = propFactory;
            _topLeftCorner = topLeftCorner;
            _tileMap = loadMapFromFile();
            _mapProps = parseMap();
        }

        public void Draw()
        {
            foreach (IDrawable prop in _mapProps)
            {
                prop.Draw();
            }
        }

        // TODO: This is a service. It needs to be in it's own class and injected in
        private char[][] loadMapFromFile(bool shouldUseVehicleTestbench = false)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = shouldUseVehicleTestbench ? "MoonTrucker.GameWorld.TestBench.txt" : "MoonTrucker.GameWorld.Map.txt";
            char[][] tileMap;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                // TODO: Remove an chars that are not in the TileType enum
                tileMap = reader.ReadToEnd()
                                 .Split("\n")
                                 .Select(line => line.ToCharArray())
                                 .ToArray();
            }

            return tileMap;
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
                        Vector2 curPosInSim = getCordInSim(curCoordinate);

                        var prop = CreatePropBodyForTile(propMapValue, propDim, PropFactory.GetOriginFromDimensions(propDim, curPosInSim));
                        if (prop != null) { props.Add(prop); }
                    }
                }
            }

            return props;
        }

        private List<IDrawable> parseMapBetter()
        {
            var props = new List<IDrawable>();

            return props;
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
                    location = getCordInSim(new MapCoordinate(randomRow, randomCol));
                }
            }

            return Vector2.Add(location, new Vector2(_tileWidt /2f, _tileWidt /2f));
        }

        private IDrawable CreatePropBodyForTile(TileType tile, Vector2 propDim, Vector2 origin)
        {
            switch (tile)
            {
                case TileType.Building:
                    return _propFactory.CreateRectangleBody(propDim.X, propDim.Y, origin);
                case TileType.Tunnel:
                    return _propFactory.CreateRectangleSensor(propDim.X, propDim.Y, origin);
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
            var simDim = dim.ToVector2() * _tileWidt;
            return simDim;
        }

        private Vector2 getCordInSim(MapCoordinate cord)
        {

            return cord.ToVector2() * _tileWidt;
        }

        #region IObserver<GameTarget> Implementation
        public virtual void Subscribe(GameTarget target)
        {
            _cancellation = target.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            _cancellation.Dispose();
        }

        public void OnCompleted()
        {
            // TODO: Remove Target
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(GameTarget target)
        {
            target.SetPosition(GetRandomTargetLocation());
        }

        #endregion
    }

    public enum TileType
    {
        Building = 'B',
        Road = '_',
        Tunnel = 'T'
    }
}
