using DefaultNamespace;
using DG.Tweening;
using Units;
using UnityEngine;

namespace Items.ItemViews
{
    public class ItemView : MonoBehaviour
    {
        private Item _item;
        public bool pickedUp;
        public string itemName;
        public Item Item => _item;


        public void SetUp(Item item)
        {
            _item = item;
            pickedUp = false;
            itemName = _item.name;
            Rotate();
        }

        public void PickUp(UnitBase unit)
        {
            if (_item is Bonus { BonusType: BonusType.Heal } bonus)
            {
                VFXController.Instance.PlayEffect(bonus.UsisngVFX, unit.Instance.transform);
                unit.UnitView.OnHit.Invoke(-bonus.Value);
                Despawn();
                return;
            }
            transform.DOMove(unit.UnitView.transform.position + new Vector3(0,1,0), 0.1f).OnComplete(() =>
            {
                ItemContainer itemContainer = new ItemContainer(Item, this, unit);
                unit.PickUpItem(itemContainer);
                
                Despawn();
            });
        }

        public void Despawn()
        {
            transform.DOKill();
            Destroy(gameObject);
        }
        

        private void Rotate()
        {
            transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 10, 0), 0.1f)
                .SetEase(Ease.InQuad)
                .SetLoops(-1, LoopType.Incremental);
        }
    }
}