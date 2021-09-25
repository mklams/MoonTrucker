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
                    char propMapValue = _tileMap[row][col];
                    // Tricky: backwards from how you think about coords in a 2d array
                    var curCoordinate = new MapCoordinate(row, col);
                    if (propMapValue == 'B' && isTopLeftCorner(curCoordinate))
                    {
                        var propDim = getPropDimensionsInSim(curCoordinate);
                        var curPosInSim = getCordInSim(curCoordinate);

                        var prop = _propFactory.CreateRectangleBody(propDim.X, propDim.Y, PropFactory.GetOriginFromDimensions(propDim, curPosInSim));
                        props.Add(prop);
                    }
                }
            }

            return props;
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
