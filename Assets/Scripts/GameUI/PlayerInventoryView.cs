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

        public event Action<ItemContainer> OnBuildingInvoked;

        private List<GameObject> itemsGo;
        private List<Button> _buttonsAttack;
        private List<Button> _buttonsDefence;
        private Unit _unit;


        public void SetUpUI(int inventoryCapacity, Unit unit)
        {
            if (_buttonsAttack != null && _buttonsAttack.Count > 0)
            {
                itemsGo.ForEach(Destroy);
            }

            _unit = unit;
            itemsGo = new List<GameObject>();
            _buttonsAttack = new List<Button>();
            _buttonsDefence = new List<Button>();

            SetUpButtons(inventoryCapacity / 2, _buttonsAttack);
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
            if (button == null)
                return;
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            OnBuildingInvoked = null;
        }

        public void PickUpItem(ItemContainer Item)
        {
            var button = Item.Item.Type switch
            {
                ItemType.ATTACK => _buttonsAttack.First(x => !x.IsActive()),
                ItemType.DEFENCE => _buttonsDefence.First(x => !x.IsActive()),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (button == null)
                return;
            button.gameObject.SetActive(true);
            button.image.sprite = Item.Item.Icon;
            button.onClick.AddListener(() =>
            {
                switch (Item.Item)
                {
                    case Bonus bonus:
                    {
                        button.onClick.RemoveAllListeners();
                        bonus.Invoke(_unit);

                        button.onClick.RemoveAllListeners();
                        button.gameObject.SetActive(false);
                        break;
                    }
                    case Building building:
                        Item.OnItemUsed += () => SwitchButton(button);
                        building.Invoke(Item);
                        OnBuildingInvoked?.Invoke(Item);
                        break;
                    case CaptureAbility ability:
                        Item.OnItemUsed += () => SwitchButton(button);
                        ability.Invoke(Item);
                        OnBuildingInvoked?.Invoke(Item);
                        break;
                    case SpecialWeapon specialWeapon:
                        Item.OnItemUsed += () => SwitchButton(button);
                        specialWeapon.Invoke(Item);
                        OnBuildingInvoked?.Invoke(Item);
                        break;
                    case SwitchingPlaces switchingPlaces:
                        Item.OnItemUsed += () => SwitchButton(button);
                        switchingPlaces.Invoke(Item);
                        OnBuildingInvoked?.Invoke(Item);
                        break;
                }
            });
        }
    }
}