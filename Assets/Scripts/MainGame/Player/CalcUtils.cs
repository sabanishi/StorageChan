using UnityEngine.UIElements;

namespace Sabanishi.MainGame
{
    public static class CalcUtils
    {
        public static Direction ReverseDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Up:
                    return Direction.Down;
            }
            return Direction.None;
        }

        public static Direction RotateDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Up;
                case Direction.Right:
                    return Direction.Down;
                case Direction.Up:
                    return Direction.Right;
            }
            return Direction.None;
        }
        
        public static Direction ReverseRotateDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Direction.Right;
                case Direction.Left:
                    return Direction.Down;
                case Direction.Right:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Left;
            }
            return Direction.None;
        }
    }
}