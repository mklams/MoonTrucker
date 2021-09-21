using System;
using Microsoft.Xna.Framework;

namespace MoonTrucker
{
    public class GameMap
    {
        private static readonly string[,] TileMap = 
        {
            { "_", "_", "_", "_", "_", "_", "_","_"},
            {"_", "B", "B" , "_", "_","B","B","_"},
            {"_", "B", "B" , "_", "_","B","B","_"},
            { "_", "_", "_", "_", "_", "_", "_","_"}
        };

        private float _tileWidt;
        private PropFactory _propFactory;
        private Vector2 _topLeftCorner;

        public GameMap(float tileWidth, PropFactory propFactory, Vector2 topLeftCorner)
        {
            _tileWidt = tileWidth;
            _propFactory = propFactory;
            _topLeftCorner = topLeftCorner;
        }

        private void parseMap()
        {
            for(int y = 0; y < TileMap.Length; y++)
            {
                for  (int x =0; x < TileMap.GetLength(1); x++)
                {

                }
            }
        }
    }

}
