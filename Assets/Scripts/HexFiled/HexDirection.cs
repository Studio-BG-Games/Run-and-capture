namespace HexFiled
{
    public enum HexDirection
    {
        NE,
        E,
        SE,
        SW,
        W,
        NW
    }

    public static class HexDirectionExtensions
    {
        public static HexDirection Back(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }

        public static HexDirection PlusSixtyDeg(this HexDirection direction)
        {
            return (int)direction < 5 ? (direction + 1) : (HexDirection)0;
        }

        public static HexDirection MinusSixtyDeg(this HexDirection direction)
        {
            return (int)direction > 0 ? (direction - 1) : (HexDirection)5;
        }

        public static HexDirection Plus120Deg(this HexDirection direction)
        {
            if ((int)direction < 4)
            {
                direction = direction + 2;
            }
            else if ((int)direction == 4)
            {
                direction = (HexDirection)0;
            }
            else
            {
                direction = (HexDirection)1;
            }
            
            return direction;
        }

        public static HexDirection Minus120Deg(this HexDirection direction)
        {
            if ((int)direction > 1)
            {
                direction = (direction - 2);
            }
            else if ((int)direction == 1)
            {
                direction = (HexDirection)5;
            }
            else
            {
                direction = (HexDirection)4;
            }

            return direction;
        }
    }
}