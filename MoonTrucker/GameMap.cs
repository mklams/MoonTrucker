using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MoonTrucker
{
    public class GameMap
    {
        private char[][] _tileMap;
        private float _tileWidt;
        private PropFactory _propFactory;
        private Vector2 _topLeftCorner;

        public GameMap(float tileWidth, PropFactory propFactory, Vector2 topLeftCorner)
        {
            loadMapFromFile();
            _tileWidt = tileWidth;
            _propFactory = propFactory;
            _topLeftCorner = topLeftCorner;
        }

        // TODO: This is a service. It needs to be in it's own class and injected in
        private void loadMapFromFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "MoonTrucker.Map.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                _tileMap = reader.ReadToEnd()
                                 .Split("\n")
                                 .Select(line => line.ToCharArray())
                                 .ToArray();
            }
        }

        public List<IDrawable> ParseMap()
        {
            var props = new List<IDrawable>();
            for (int row = 0; row < _tileMap.Length; row++)
            {
                for (int col = 0; col < _tileMap[row].Length; col++)
                {
                    var propMapValue = (TileType)_tileMap[row][col];
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

        private IDrawable CreatePropBodyForTile(TileType tile, Vector2 propDim, Vector2 origin)
        {
            switch(tile)
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
    }

    public enum TileType
    {
        Building = 'B',
        Road = '_',
        Tunnel = 'T'
    }

    public class MapCoordinate
    {
        public int Row;
        public int Column;

        public MapCoordinate() { }
        public MapCoordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Vector2 ToVector2()
        {
            // Flip row and column order to match X/Y 
            return new Vector2(Column, Row);
        }
    }

}
