using System;
using System.Diagnostics;
using Controller;
using Data;
using DefaultNamespace;
using DG.Tweening;
using GameUI;
using HexFiled;
using Items;
using Runtime.Controller;
using Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chars
{
    public class PlayerControl : IFixedExecute, IDisposable
    {
        private Unit _unit;
        private Joystick _moveJoystick;
        private Joystick _attackJoystick;
        private Joystick _placeJoystick;
        private UnitView _unitView;
        private Vector2 _attackDircetion;
       
        private PlayerInventoryView _inventoryView;
        private Item _itemToPlace;
        private HexCell _cellToPlace;


        public PlayerControl(Unit unit, PlayerControlView joyView, PlayerInventoryView inventoryView)
        {
            _unit = unit;
            _moveJoystick = joyView.MoveJoystick;
            _attackJoystick = joyView.AttackJoystick;
            _placeJoystick = joyView.PlaceJoystick;
            _placeJoystick.gameObject.SetActive(false);
            _unitView = unit.UnitView;

            _attackJoystick.OnTouchUp += DoAttack;
            _attackJoystick.OnDrug += AimCanvas;

            inventoryView.SetUpUI(unit.InventoryCapacity);
            _unit.OnItemPickUp += PickUp;
            _inventoryView = inventoryView;
            inventoryView.OnBuildingInvoked += AimPlaceItem;

            _placeJoystick.OnDrug += PlaceItemAim;
            _placeJoystick.OnTouchUp += PlaceItem;
        }

        private void AimPlaceItem(Item item)
        {
            if (_unit.IsBusy || !_unit.IsAlive) return;
            _attackJoystick.gameObject.SetActive(false);
            _placeJoystick.gameObject.SetActive(true);
            _itemToPlace = item;
        }

        private void PlaceItem()
        {
            if(!_unit.IsAlive) return;
            switch (_itemToPlace)
            {
                case Building building:
                    _unitView.AimCanvas.SetActive(false);
                    _placeJoystick.gameObject.SetActive(false);
                    if (_cellToPlace == null)
                    {
                        return;
                    }

                    building.PlaceItem(_cellToPlace);
                    break;
                case CaptureAbility ability:
                    ability.UseAbility();
                    _placeJoystick.gameObject.SetActive(false);
                    break;
            }
            _attackJoystick.gameObject.SetActive(true);
        }

        private void PickUp(Item item)
        {
            _inventoryView.PickUpItem(item);
        }

        private void DoAttack()
        {
            _unitView.AimCanvas.SetActive(false);
            _unit.StartAttack();
        }

        private void AimCanvas(Vector2 attackDir)
        {
            if (_unit.IsBusy || !_unit.IsAlive) return;
            
            _unitView.AimCanvas.SetActive(true);
            _unit.Aim(attackDir);
        }

        private void PlaceItemAim(Vector2 placeDir)
        {
            if (_unit.IsBusy || !_unit.IsAlive) return;

            switch (_itemToPlace)
            {
                case Building building:
                    _unitView.AimCanvas.SetActive(true);
                    _cellToPlace = _unit.PlaceItemAim(DirectionHelper.VectorToDirection(placeDir));
                    break;
                case CaptureAbility ability:
                    ability.Aim(DirectionHelper.VectorToDirection(placeDir));
                    break;
            }
            
           
        }

        public void FixedExecute()
        {
            if (!_unit.IsBusy && _unit.IsAlive && _moveJoystick.Direction != Vector2.zero)
            {
                _placeJoystick.gameObject.SetActive(false);
                _unit.Move(DirectionHelper.VectorToDirection(_moveJoystick.Direction.normalized));
            }
        }

        public void Dispose()
        {
            _attackJoystick.OnTouchUp -= DoAttack;
            _attackJoystick.OnDrug -= AimCanvas;
            _placeJoystick.OnDrug -= PlaceItemAim;
            _placeJoystick.OnTouchUp -= PlaceItem;
        }
    }
}