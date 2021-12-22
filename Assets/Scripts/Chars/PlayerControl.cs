using System;
using Controller;
using Data;
using HexFiled;
using Runtime.Controller;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chars
{
    public class PlayerControl : IFixedExecute, IExecute
    {
        private Player _player;
        private FloatingJoystick _moveJoystick;
        private FloatingJoystick _attackJoystick;
        private Camera _camera;
        private Vector2 _attackDircetion;


        public PlayerControl(Player player, PlayerData playerData)
        {
            _player = player;
            var joyView = Object.Instantiate(playerData.joystickView);
            _moveJoystick = joyView.MoveJoystick;
            _attackJoystick = joyView.AttackJoystick;
            _camera = Camera.main;
            _attackJoystick.OnTouchUp += DoAttack;
        }

        private void DoAttack()
        {
            _player.Attack(_attackDircetion);
        }
        
        public void FixedExecute()
        {
            if (!_player.IsBusy && _moveJoystick.Direction != Vector2.zero)
            {
                _player.Move(VectorToDirection(_moveJoystick.Direction.normalized));
            }

            if (!_player.IsBusy && _attackJoystick.isPressed)
            {
                _attackDircetion = _attackJoystick.Direction.normalized;
                _player.Aim(_attackDircetion);
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
            if (_player.UnitView != null)
            {
                _player.UnitView.charBarCanvas.transform.LookAt(
                    _player.UnitView.charBarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                    _camera.transform.rotation * Vector3.up);
            }
        }
    }
}