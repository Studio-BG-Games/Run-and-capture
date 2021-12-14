using HexFiled;

namespace Chars
{
    public interface IUnit
    {
        public void Move(HexCoordinates coordinates);
        public void Spawn();
        public void Death();
    }
}