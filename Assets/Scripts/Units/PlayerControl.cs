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
        private HexDirection previousDir;

        private bool returnedMoveJoystick = false;

        private PlayerInventoryView _inventoryView;
        private Item _itemToPlace;
        private HexCell _cellToPlace;

        private Unit chosenUnit;
        private int _aimCount = 0;

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

            inventoryView.SetUpUI(unit.InventoryCapacity, unit);
            _unit.OnItemPickUp += PickUp;
            _inventoryView = inventoryView;
            inventoryView.OnBuildingInvoked += AimPlaceItem;

            _placeJoystick.OnDrug += AbilityAim;
            _placeJoystick.OnTouchUp += UseAbility;
        }

        private void AimPlaceItem(Unit unit, Item item)
        {
            if (unit.IsBusy || !unit.IsAlive) return;
            _attackJoystick.gameObject.SetActive(false);
            _placeJoystick.gameObject.SetActive(true);
            _itemToPlace = item;
        }

        private void UseAbility()
        {
            if (!_unit.IsAlive) return;
            _attackJoystick.gameObject.SetActive(true);
            _placeJoystick.gameObject.SetActive(false);
            if (_aimCount == 2)
            {
                return;
            }

            switch (_itemToPlace)
            {
                case Building building:
                    _unitView.AimCanvas.SetActive(false);
                    if (_cellToPlace == null)
                    {
                        return;
                    }

                    building.PlaceItem(_cellToPlace, _unit);
                    break;
                case CaptureAbility ability:
                    ability.UseAbility(_unit);
                    break;
                case SpecialWeapon weapon:
                    weapon.Fire(_unit);
                    break;
                case SwitchingPlaces switchingPlaces:
                    switchingPlaces.UseAbility(_unit, chosenUnit);
                    break;
            }
        }

        private void PickUp(Item item)
        {
            _inventoryView.PickUpItem(item);
        }

        private void DoAttack()
        {
            _unitView.AimCanvas.SetActive(false);
            if (_aimCount == 0 || _aimCount > 1)
                _unit.StartAttack();
            _aimCount = 0;
        }

        private void AimCanvas(Vector2 attackDir)
        {
            if (_unit.IsBusy || !_unit.IsAlive) return;
            if (attackDir.Equals(Vector2.zero))
            {
                _unitView.AimCanvas.SetActive(false);
                _unit.Aim(attackDir);
                if (_aimCount > 0)
                {
                    _aimCount = 1;
                }

                return;
            }

            _aimCount++;
            _unitView.AimCanvas.SetActive(true);
            _unit.Aim(attackDir);
        }

        private void AbilityAim(Vector2 placeDir)
        {
            if (_unit.IsBusy || !_unit.IsAlive) return;

            if (placeDir.Equals(Vector2.zero))
            {
                _aimCount = -1;
            }

            switch (_itemToPlace)
            {
                case Building building:
                    if (_aimCount == -1)
                    {
                        _aimCount = 2;
                        _unitView.AimCanvas.SetActive(false);
                        return;
                    }

                    _unitView.AimCanvas.SetActive(true);
                    _cellToPlace = _unit.PlaceItemAim(DirectionHelper.VectorToDirection(placeDir.normalized));
                    _aimCount = 1;
                    break;
                case CaptureAbility ability:
                    if (_aimCount == -1)
                    {
                        _aimCount = 2;
                        ability.DeAim();
                        return;
                    }
                    ability.Aim(DirectionHelper.VectorToDirection(placeDir.normalized), _unit);
                    _aimCount = 1;
                    break;
                case SpecialWeapon weapon:
                    weapon.Aim(DirectionHelper.VectorToDirection(placeDir.normalized), _unit);
                    break;
                case SwitchingPlaces switchingPlaces:
                    switchingPlaces.Aim(placeDir.normalized, _unit, out var unit);
                    if (unit != null)
                    {
                        chosenUnit = unit;
                    }
                    break;
            }
        }

        public void FixedExecute()
        {
            if ((previousDir != DirectionHelper.VectorToDirection(_moveJoystick.Direction.normalized) ||
                 _moveJoystick.isJoysticDirectionZero) && _unit.IsHardToCapture)
            {
                returnedMoveJoystick = _unit.IsHardToCapture;
            }

            if (!_unit.IsAlive || _moveJoystick.Direction == Vector2.zero) return;

            if (_placeJoystick.gameObject.activeSelf)
            {
                _placeJoystick.gameObject.SetActive(false);
                switch (_itemToPlace)
                {
                    case CaptureAbility ability:
                        ability.DeAim();
                        break;
                    case Building building:
                        _unitView.AimCanvas.SetActive(false);
                        break;
                    case SpecialWeapon weapon:
                        weapon.DeAim();
                        break;
                    case SwitchingPlaces place:
                        place.DeAim();
                        break;
                    
                }
            }

            if (!_attackJoystick.gameObject.activeSelf)
                _attackJoystick.gameObject.SetActive(true);

            if (_unit.IsHardToCapture && returnedMoveJoystick)
            {
                _unit.Retreet(DirectionHelper.VectorToDirection(_moveJoystick.Direction.normalized));
                returnedMoveJoystick = false;
            }
            else
            {
                _unit.Move(DirectionHelper.VectorToDirection(_moveJoystick.Direction.normalized));
            }

            previousDir = DirectionHelper.VectorToDirection(_moveJoystick.Direction.normalized);
        }

        public void Dispose()
        {
            _attackJoystick.OnTouchUp -= DoAttack;
            _attackJoystick.OnDrug -= AimCanvas;
            _placeJoystick.OnDrug -= AbilityAim;
            _placeJoystick.OnTouchUp -= UseAbility;
        }
    }
}