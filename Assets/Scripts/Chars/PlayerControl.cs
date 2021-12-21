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


        public PlayerControl(Player player, PlayerData playerData)
        {
            _player = player;
            var joyView = Object.Instantiate(playerData.joystickView);
            _moveJoystick = joyView.MoveJoystick;
            _attackJoystick = joyView.AttackJoystick;
            _camera = Camera.main;
            
        }


        public void FixedExecute()
        {
            
            if (!_player.IsBusy && _moveJoystick.Direction != Vector2.zero)
            {
                _player.Move(VectorToDirection(_moveJoystick.Direction.normalized));
               
            }

            if (!_player.IsBusy && _attackJoystick.Direction != Vector2.zero)
            {
                _player.Attack(VectorToDirection(_attackJoystick.Direction.normalized));
               
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
            _player.PlayerView.charBarCanvas.transform.LookAt(
                _player.PlayerView.charBarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                _camera.transform.rotation * Vector3.up);
        }
    }
}