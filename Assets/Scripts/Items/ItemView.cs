﻿using System;
using DG.Tweening;
using Units;
using UnityEngine;

namespace Items
{
    public class ItemView : MonoBehaviour
    {
        private Item _item;

        public Item Item => _item;

        public void SetUp(Item item)
        {
            _item = item;
            Rotate();
        }

        public ItemView PickUp(Unit unit)
        {
            transform.DOKill();
            _item.PickUp(unit);
            return this;
        }

        private void Rotate()
        {
            
            transform.DORotate(transform.rotation.eulerAngles + new Vector3(0,10,0), 0.1f)
                .SetEase(Ease.Linear)
                .OnComplete(Rotate);
        }
    }
}