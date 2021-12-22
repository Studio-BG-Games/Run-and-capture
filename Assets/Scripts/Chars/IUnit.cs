
using HexFiled;
using UnityEngine;

namespace Chars
{
    public interface IUnit
    {
        public void Move(HexDirection direction);
        public void Spawn();
        public void Death();
        public void Attack(Vector2 direction);
        public void Damage(float dmg);
    }
}