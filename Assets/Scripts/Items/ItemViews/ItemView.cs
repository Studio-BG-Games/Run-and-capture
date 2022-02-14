using DG.Tweening;
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

        private void OnDestroy()
        {
            transform.DOKill();
        }

        private void Rotate()
        {
            transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 10, 0), 0.1f)
                .SetEase(Ease.InQuad)
                .SetLoops(-1, LoopType.Incremental);
        }
    }
}