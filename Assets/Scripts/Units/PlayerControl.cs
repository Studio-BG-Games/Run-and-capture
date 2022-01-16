using System;
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
    public class PlayerControl : IFixedExecute, IExecute
    {
        private Unit _unit;
        private Joystick _moveJoystick;
        private Joystick _attackJoystick;
        private Joystick _placeJoystick;
        private Camera _camera;
        private Vector2 _attackDircetion;
        private HexDirection _placeDirection;
        private PlayerInventoryView _inventoryView;
        private Building _itemToPlace;
        private HexCell _cellToPlace;
        


        public PlayerControl(Unit unit, PlayerControlView joyView, PlayerInventoryView inventoryView)
        {
            _unit = unit;
            _moveJoystick = joyView.MoveJoystick;
            _attackJoystick = joyView.AttackJoystick;
            _placeJoystick = joyView.PlaceJoystick;
            _placeJoystick.gameObject.SetActive(false);
            _camera = Camera.main;
            
            _attackJoystick.OnTouchUp += DoAttack;
            _attackJoystick.OnDrug += AimCanvas;
            
            inventoryView.SetUpUI(unit.InventoryCapacity);
            _unit.OnItemPickUp += PickUp;
            _inventoryView = inventoryView;
            inventoryView.OnBuildingInvoked += AimPlaceItem;
            
            _placeJoystick.OnDrug += PlaceItemAim;
            _placeJoystick.OnTouchUp += PlaceItem;

        }

        private void AimPlaceItem(Building item)
        {
            if (!_unit.IsBusy)
            {
                _placeJoystick.gameObject.SetActive(true);
                _itemToPlace = item;
            }
        }

        private void PlaceItem()
        {
            _unit.UnitView.AimCanvas.SetActive(false);
            
            _itemToPlace.PlaceItem(_cellToPlace);
            
            _placeJoystick.gameObject.SetActive(false);
        }
        private void PickUp(Item item)
        {
            _inventoryView.PickUpItem(item);
        }
        private void DoAttack()
        {
            _unit.UnitView.AimCanvas.SetActive(false);
            _unit.StartAttack();
        }
        
        private void AimCanvas(Vector2 attackDir)
        {
            if (!_unit.IsBusy)
            {
                _unit.UnitView.AimCanvas.SetActive(true);
                _unit.Aim(attackDir);
            }
        }

        private void PlaceItemAim(Vector2 placeDir)
        {
            if (!_unit.IsBusy)
            {
                _cellToPlace = _unit.PlaceItemAim(DirectionHelper.VectorToDirection(placeDir.normalized));
            }
        }

        public void FixedExecute()
        {
            
            if (!_unit.IsBusy && _moveJoystick.Direction != Vector2.zero)
            {
                _placeJoystick.gameObject.SetActive(false);
                _unit.Move(DirectionHelper.VectorToDirection(_moveJoystick.Direction.normalized));
            }
            
        }

       

        public void Execute()
        {
            if (_unit.IsAlive)
            {
                _unit.UnitView.BarCanvas.transform.LookAt(
                    _unit.UnitView.BarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                    _camera.transform.rotation * Vector3.up);
            }
        }
    }
}