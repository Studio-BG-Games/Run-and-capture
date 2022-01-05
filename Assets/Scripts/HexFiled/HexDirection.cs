namespace HexFiled
{
    public enum HexDirection {
        NE, E, SE, SW, W, NW
    }

    public static class HexDirectionExtensions
    {

        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
        
        public static HexDirection PlusSixtyDeg(this HexDirection direction)
        {
            return (int) direction < 5? (direction + 1) : (HexDirection)0;
        }
        
        public static HexDirection MinusSixtyDeg(this HexDirection direction)
        {
            return (int)direction > 0 ? (direction - 1) : (HexDirection)5;
        }
    }
}