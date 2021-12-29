using System;
using Controller;
using Data;
using DG.Tweening;
using HexFiled;
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
        private Camera _camera;
        private Vector2 _attackDircetion;


        public PlayerControl(Unit unit, PlayerControlView joyView)
        {
            _unit = unit;
            _moveJoystick = joyView.MoveJoystick;
            _attackJoystick = joyView.AttackJoystick;
            _camera = Camera.main;
            _attackJoystick.OnTouchUp += DoAttack;
            _attackJoystick.OnTouchDown += AimCanvas;
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