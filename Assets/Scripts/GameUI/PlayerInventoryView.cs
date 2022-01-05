using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryView : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject grid;

    public Action<Item> OnItemInvoked;
    
    private List<Button> _buttons;
    private Queue<Button> _freeButtons;
    private Dictionary<Item, Button> _dictionary;


    public void SetUpUI(int inventoryCapacity)
    {
        _buttons = new List<Button>();
        _dictionary = new Dictionary<Item, Button>();
        _freeButtons = new Queue<Button>();
        for (int i = 0; i < inventoryCapacity; i++)
        {
            var itemGo = Instantiate(item, grid.transform);
            var button = itemGo.GetComponentInChildren<Button>();
            _buttons.Add(button);
            button.gameObject.SetActive(false);
        }
        
        _buttons.ForEach(button => _freeButtons.Enqueue(button));
    }

    private void SwitchButton(Item item)
    {
        var button = _dictionary[item];
        _dictionary.Remove(item);
        button.gameObject.SetActive(false);
        _freeButtons.Enqueue(button);
    }
    
    public void PickUpItem(Item item)
    {
        var button = _freeButtons.Dequeue();
        _dictionary.Add(item, button);
        button.gameObject.SetActive(true);
        button.image.sprite = item.Icon;
        button.onClick.AddListener(() =>
        {
            if (item.IsInstantUse)
            {
                item.InstanceInvoke();
                _freeButtons.Enqueue(button);
                button.gameObject.SetActive(false);
            }
            else
            {
                item.Invoke(SwitchButton);
                OnItemInvoked?.Invoke(item);
            }

        });
        
        
    }
}