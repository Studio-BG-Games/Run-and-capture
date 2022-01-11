using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class PlayerInventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject item;
        [SerializeField] private GameObject grid;

        public Action<Building> OnBuildingInvoked;

        private List<Button> _buttons;
        private Button[] _freeButtons;
        private Dictionary<Item, Button> _dictionary;


        public void SetUpUI(int inventoryCapacity)
        {
            _buttons = new List<Button>();
            _dictionary = new Dictionary<Item, Button>();
            _freeButtons = new Button[inventoryCapacity];
            for (int i = 0; i < inventoryCapacity; i++)
            {
                var itemGo = Instantiate(item, grid.transform);
                var button = itemGo.GetComponentInChildren<Button>();
                _buttons.Add(button);
                button.gameObject.SetActive(false);
            }

            var j = 0;
            _buttons.ForEach(button => _freeButtons[j++] = button);
        }

        private void SwitchButton(Item item)
        {
            var button = _dictionary[item];
            _dictionary.Remove(item);
            button.gameObject.SetActive(false);
            for (int i = 0; i < _freeButtons.Length; i++)
            {
                if (_freeButtons[i] != null) continue;
                _freeButtons[i] = button;
                break;
            }
        }

        public void PickUpItem(Item item)
        {
            Button button = null;
            for (int i = 0; i < _freeButtons.Length; i++)
            {
                if (_freeButtons[i] == null) continue;
                button = _freeButtons[i];
                _freeButtons[i] = null;
                break;
            }

            if (button == null)
                return;
            _dictionary.Add(item, button);
            button.gameObject.SetActive(true);
            button.image.sprite = item.Icon;
            button.onClick.AddListener(() =>
            {
                switch (item)
                {
                    case Bonus _bonus:
                    {
                        button.onClick.RemoveAllListeners();
                        _bonus.Invoke();
                        for (int i = 0; i < _freeButtons.Length; i++)
                        {
                            if (_freeButtons[i] != null) continue;
                            _freeButtons[i] = button;
                            break;
                        }
                        button.onClick.RemoveAllListeners();
                        button.gameObject.SetActive(false);
                        break;
                    }
                    case Building _building:
                        _building.Invoke(SwitchButton);
                        OnBuildingInvoked?.Invoke(_building);
                        break;
                }
            });
        }
    }
}