using System;
using DG.Tweening;
using Units;
using UnityEngine;

namespace Items
{
    public class ItemView : MonoBehaviour
    {
        private Item _item;
        public bool pickedUp;
        public string itemName;
        public Item Item => _item;

        
        private void Start()
        {
            pickedUp = false;
            itemName = _item.name;
        }

        public void SetUp(Item item)
        {
            _item = item;
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