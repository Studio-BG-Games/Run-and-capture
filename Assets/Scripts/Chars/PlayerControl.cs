﻿using System;
using Data;
using HexFiled;
using Runtime.Controller;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chars
{
    public class PlayerControl : IExecute
    {
        private Player _player;
        private FloatingJoystick _moveJoystick;
        private FloatingJoystick _attackJoystick;
        private float _curTime;
        private float _tick;

        public PlayerControl(Player player, PlayerData playerData)
        {
            _player = player;
            var joyView = Object.Instantiate(playerData.joystickView);
            _moveJoystick = joyView.MoveJoystick;
            _attackJoystick = joyView.AttackJoystick;
            _curTime = Time.time;
            _tick = playerData.Tick;
        }

        public void Execute()
        {
            
            if (Time.time - _curTime >= _tick && _moveJoystick.Direction != Vector2.zero)
            {
                _curTime = Time.time;
                _player.Move(VectorToDirection(_moveJoystick.Direction.normalized));
                
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
    }
}