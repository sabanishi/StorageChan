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
                default:
                    break;
            }
            return Direction.None;
        }
    }
}