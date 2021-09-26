using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
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