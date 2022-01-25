using System;
using System.Collections.Generic;
using Items;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class PlayerInventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject item;
        [SerializeField] private GameObject grid;

        public event Action<Item> OnBuildingInvoked;

        private List<GameObject> itemsGo; 
        private List<Button> _buttons;
        private Button[] _freeButtons;
        private Dictionary<Button, Item> _dictionary;


        public void SetUpUI(int inventoryCapacity)
        {
            _dictionary = new Dictionary<Button, Item>();
            if (_buttons != null && _buttons.Count > 0)
            {
               itemsGo.ForEach(Destroy);
            }

            itemsGo = new List<GameObject>();
            _buttons = new List<Button>();
            
            _freeButtons = new Button[inventoryCapacity];
            for (int i = 0; i < inventoryCapacity; i++)
            {
                var itemGo = Instantiate(item, grid.transform);
                itemsGo.Add(itemGo);
                var button = itemGo.GetComponentInChildren<Button>();
                _buttons.Add(button);
                _dictionary.Add(button, null);
                button.gameObject.SetActive(false);
            }

            var j = 0;
            _buttons.ForEach(button => _freeButtons[j++] = button);
        }

        private void SwitchButton(Button button)
        {
            button.onClick.RemoveAllListeners();
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
            _dictionary[button] = item;
            button.gameObject.SetActive(true);
            button.image.sprite = item.Icon;
            button.onClick.AddListener(() =>
            {
                switch (item)
                {
                    case Bonus bonus:
                    {
                        button.onClick.RemoveAllListeners();
                        bonus.Invoke();
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
                    case Building building:
                        building.Invoke(() => SwitchButton(button));
                        OnBuildingInvoked?.Invoke(building);
                        break;
                    case CaptureAbility ability:
                        ability.Invoke(() => SwitchButton(button));
                        OnBuildingInvoked?.Invoke(ability);
                        break;
                }
            });
        }
    }
}