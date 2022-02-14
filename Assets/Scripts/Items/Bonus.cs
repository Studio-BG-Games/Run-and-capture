using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;

namespace Items
{
    public enum BonusType
    {
        Attack,
        Defence,
        Heal,
        Magnet,
        Mana
    }

    [CreateAssetMenu(fileName = "BonusItem", menuName = "Item/Bonus")]
    public class Bonus : Item
    {
        [SerializeField] private float duration;
        [SerializeField] private int value;
        [SerializeField] private BonusType bonusType;
        [SerializeField] private GameObject usisngVFX;

        public BonusType BonusType => bonusType;
        

        public int Value => value;

        public GameObject UsisngVFX => usisngVFX;


        public void Invoke(Unit unit)
        {
            unit.SetUpBonus(duration, value, bonusType);
            var vfx = VFXController.Instance.PlayEffect(usisngVFX, unit.Instance.transform);
            TimerHelper.Instance.StartTimer(() => Destroy(vfx), duration);
            unit.UseItem(this);
        }
    }
}