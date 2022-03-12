using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units;
using Units.Views;
using UnityEngine;

namespace Items.ItemViews
{
    public class ArrowView : MonoBehaviour, ISetUp
    {
        [SerializeField] private float scale;
        [SerializeField] private float duration;
        [SerializeField]private UnitColor color;

        public void SetUp(UnitBase unit)
        {
            color = unit.Color;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var unit = collision.gameObject.GetComponent<UnitView>();

            if (unit != null && unit.Color != color)
            {
                unit.Unit.SetTimeScale(scale);
                TimerHelper.Instance.StartTimer(() => {unit.Unit.SetTimeScale(1f);}, duration);
                transform.DOKill();
                Destroy(gameObject);
            }
        }
    }
}
