using System;
using Controller;
using Data;
using DG.Tweening;
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
        private Item _itemToPlace;
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
            inventoryView.OnItemInvoked += AimPlaceItem;
            _placeJoystick.OnTouchDown += AimCanvas;
            _placeJoystick.OnTouchUp += PlaceItem;

        }

        private void AimPlaceItem(Item item)
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
                
                _unit.Move(VectorToDirection(_moveJoystick.Direction.normalized));
            }

            if (!_unit.IsBusy && _attackJoystick.isPressed)
            {
                _attackDircetion = _attackJoystick.Direction.normalized;
                _unit.Aim(_attackDircetion);
            }

            if (!_unit.IsBusy && _placeJoystick.isPressed)
            {
                _placeDirection = VectorToDirection(_placeJoystick.Direction.normalized);
                _cellToPlace = _unit.PlaceItemAim(_placeDirection);
            }
        }

        private static HexDirection VectorToDirection(Vector2 dir)
        {
            if (dir.x >= 0 && dir.y <= 1 && dir.x <= 1 && dir.y >= 0.5)
            {
                return HexDirection.NE;
            }

            if (Math.Abs(dir.x - 1f) < 0.2 && dir.y <= 0.5 && dir.y >= -0.5)
            {
                return HexDirection.E;
            }

            if (dir.x <= 1 && dir.y <= -0.5 && dir.x >= 0 && dir.y >= -1)
            {
                return HexDirection.SE;
            }

            if (dir.x <= 0 && dir.y >= -1 && dir.x >= -1 && dir.y <= -0.5)
            {
                return HexDirection.SW;
            }

            if (Math.Abs(dir.x - (-1f)) < 0.2 && dir.y >= -0.5 && dir.y <= 0.5)
            {
                return HexDirection.W;
            }

            if (dir.x >= -1 && dir.y >= 0.5 && dir.x <= 0 && dir.y <= 1)
            {
                return HexDirection.NW;
            }

            return HexDirection.W;
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