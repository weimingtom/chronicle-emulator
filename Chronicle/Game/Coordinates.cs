using System;

namespace Chronicle.Game
{
    public class Coordinates
    {
        private short mX;
        private short mY;

        public Coordinates(short pX, short pY) { mX = pX; mY = pY; }

        public short X { get { return mX; } set { mX = value; } }
        public short Y { get { return mY; } set { mY = value; } }

        public static int operator -(Coordinates pCoordinates1, Coordinates pCoordinates2)
        {
            return (int)Math.Sqrt(Math.Pow(pCoordinates1.X - pCoordinates2.X, 2) + Math.Pow(pCoordinates1.Y - pCoordinates2.Y, 2));
        }
    }
}
