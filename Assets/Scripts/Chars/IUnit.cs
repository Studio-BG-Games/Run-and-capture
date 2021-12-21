using HexFiled;

namespace Chars
{
    public interface IUnit
    {
        public void Move(HexDirection direction);
        public void Spawn();
        public void Death();
        public void Attack(HexDirection direction);
        public void Damage(float dmg);
    }
}