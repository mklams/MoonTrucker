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

        // TODO: Use a more generic type than RectangleProp
        public List<RectangleProp> ParseMap()
        {
            var props = new List<RectangleProp>();
            for (int row = 0; row < _tileMap.Length; row++)
            {
                for (int col = 0; col < _tileMap[row].Length; col++)
                {
                    char propMapValue = _tileMap[row][col];
                    // Tricky: backwards from how you think about coords in a 2d array
                    var curPos = new Point(col, row);
                    if (propMapValue == 'B' && isTopLeftCorner(curPos))
                    {
                        var propDim = getPropDimensionsInSim(curPos);
                        var curPosInSim = getCordInSim(curPos);

                        var prop = _propFactory.CreateRectangleBody(propDim.X, propDim.Y, PropFactory.GetOriginFromDimensions(propDim, curPosInSim));
                        props.Add(prop);
                    }
                }
            }

            return props;
        }

        private bool isTopLeftCorner(Point position)
        {
            int colPos = position.X;
            int rowPos = position.Y;
            char propMapValue = _tileMap[rowPos][colPos];

            return !doesTilePosMatchValue(propMapValue, new Point(rowPos - 1, colPos)) && !doesTilePosMatchValue(propMapValue, new Point(rowPos, colPos - 1));
        }

        private bool doesTilePosMatchValue(char value, Point position)
        {
            if (position.X < 0 || position.Y < 0)
            {
                return false;
            }

            return _tileMap[position.X][position.Y] == value;
        }

        // Get dimensions in array index
        private Point getPropDimensions(Point startingPosition)
        {
            int width = 0;
            int height = 0;
            //Trick: col and x/y are fliped
            int rowPos = startingPosition.Y;
            int colPos = startingPosition.X;
            char propMapValue = _tileMap[rowPos][colPos];
            for (int curXPos = rowPos; curXPos < _tileMap.Length; curXPos++)
            {
                if (_tileMap[curXPos][colPos] != propMapValue)
                {
                    break;
                }
                width++;
            }

            for (int curYPos = colPos; curYPos < _tileMap[rowPos].Length; curYPos++)
            {
                if (_tileMap[rowPos][curYPos] != propMapValue)
                {
                    break;
                }
                height++;
            }

            return new Point(height, width);
        }

        private Vector2 getPropDimensionsInSim(Point startingPosition)
        {
            var dim = getPropDimensions(startingPosition);
            var simDim = dim.ToVector2() * _tileWidt;
            return simDim;
        }

        private Vector2 getCordInSim(Point cord)
        {
            return cord.ToVector2() * _tileWidt;
        }
    }

}
