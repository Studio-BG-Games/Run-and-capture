using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusSlot : MonoBehaviour
{
    public Image icon;
    //public Button removeBtn;

    private Bonus _item;

    public void Additem(Bonus newItem)
    {
        _item = newItem;
        icon.sprite = _item.icon;
        icon.enabled = true;

        //removeBtn.interactable = true;
    }

    public void ClearSlot()
    {
        _item = null;
        icon.sprite = null;
        icon.enabled = false;

       // removeBtn.interactable = false;
    }

    /*public void OnRemoveBtn()
    {
        Inventory.instance.Remove(item);
    }*/

    public void UseItem()
    {
        if (_item != null)
        {
            _item.Use();
        }
    }
}
