using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Button> _buttonsDefence;


        public void SetUpUI(int inventoryCapacity)
        {
            if (_buttons != null && _buttons.Count > 0)
            {
                itemsGo.ForEach(Destroy);
            }

            itemsGo = new List<GameObject>();
            _buttons = new List<Button>();
            _buttonsDefence = new List<Button>();

            SetUpButtons(inventoryCapacity / 2, _buttons);
            SetUpButtons(inventoryCapacity / 2, _buttonsDefence);
        }

        private void SetUpButtons(int count, List<Button> buttons)
        {
            for (int i = 0; i < count; i++)
            {
                var itemGo = Instantiate(item, grid.transform);
                itemsGo.Add(itemGo);
                var button = itemGo.GetComponentInChildren<Button>();
                buttons.Add(button);
                button.gameObject.SetActive(false);
            }
        }

        private void SwitchButton(Button button)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        public void PickUpItem(Item Item)
        {
            var button = Item.Type switch
            {
                ItemType.ATTACK => _buttons.First(x => !x.IsActive()),
                ItemType.DEFENCE => _buttonsDefence.First(x => !x.IsActive()),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (button == null)
                return;
            button.gameObject.SetActive(true);
            button.image.sprite = Item.Icon;
            button.onClick.AddListener(() =>
            {
                switch (Item)
                {
                    case Bonus bonus:
                    {
                        button.onClick.RemoveAllListeners();
                        bonus.Invoke();

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
                    case SpecialWeapon specialWeapon:
                        specialWeapon.Invoke(() => SwitchButton(button));
                        OnBuildingInvoked?.Invoke(specialWeapon);
                        break;
                }
            });
        }
    }
}