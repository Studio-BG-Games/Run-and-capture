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
        private FloatingJoystick _moveJoystick;
        private FloatingJoystick _attackJoystick;
        private FloatingJoystick _placeJoystick;
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
            _attackJoystick.OnTouchDown += AimCanvas;
            
            inventoryView.SetUpUI(unit.InventoryCapacity);
            _unit.OnItemPickUp += PickUp;
            _inventoryView = inventoryView;
            inventoryView.OnBuildingInvoked += AimPlaceItem;
            _placeJoystick.OnTouchDown += AimCanvas;
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
        
        private void AimCanvas()
        {
            if (!_unit.IsBusy)
                _unit.UnitView.AimCanvas.SetActive(true);
        }

        public void FixedExecute()
        {
            
            if (!_unit.IsBusy && _moveJoystick.Direction != Vector2.zero)
            {
                _placeJoystick.gameObject.SetActive(false);
                _unit.Move(DirectionHelper.VectorToDirection(_moveJoystick.Direction.normalized));
            }

            if (!_unit.IsBusy && _attackJoystick.isPressed)
            {
                _attackDircetion = _attackJoystick.Direction.normalized;
                _unit.Aim(_attackDircetion);
            }

            if (!_unit.IsBusy && _placeJoystick.isPressed)
            {
                _placeDirection = DirectionHelper.VectorToDirection(_placeJoystick.Direction.normalized);
                _cellToPlace = _unit.PlaceItemAim(_placeDirection);
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