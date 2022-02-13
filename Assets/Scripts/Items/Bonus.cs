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
        Magnet
    }

    [CreateAssetMenu(fileName = "BonusItem", menuName = "Item/Bonus")]
    public class Bonus : Item
    {
        [SerializeField] private float duration;
        [SerializeField] private int value;
        [SerializeField] private BonusType bonusType;
        [SerializeField] private GameObject usisngVFX;

        public BonusType BonusType => bonusType;

        public override void PickUp(Unit unit)
        {
            if(bonusType != BonusType.Heal)
                base.PickUp(unit);
            else
            {
                VFXController.Instance.PlayEffect(usisngVFX, Unit.Instance.transform);
                Unit.UnitView.OnHit.Invoke(-value);
                Despawn();
            }
        }

        public void Invoke(Unit unit)
        {
            unit.SetUpBonus(duration, value, bonusType);
            var vfx = VFXController.Instance.PlayEffect(usisngVFX, unit.Instance.transform);
            TimerHelper.Instance.StartTimer(() => Destroy(vfx), duration);
            unit.UseItem(this);
        }
    }
}