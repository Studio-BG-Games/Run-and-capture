﻿using System.Collections.Generic;
using System.Linq;
using CamControl;
using Controller;
using Data;
using DefaultNamespace.AI;
using GameUI;
using HexFiled;
using Runtime.Controller;
using Units;
using UnityEngine;
using Weapons;

namespace Chars
{
    public class UnitFactory
    {
        private readonly HexGrid _hexGrid;
        private readonly Weapon _chosenWeapon;
        private readonly Data.Data _data;
        private readonly Controllers _controllers;
        private readonly UIController _uiController;
        public Unit Player { get; private set; }

        public UnitFactory(HexGrid grid, Data.Data data, UIController uiController, PaintedController paintedController,
            Controllers controllers)
        {
            _hexGrid = grid;
            _data = data;
            _chosenWeapon = data.ChosenWeapon;
            _uiController = uiController;
            _controllers = controllers;
        }

        public void SpawnList(List<UnitInfo> units)
        {
            units.ForEach(x => Spawn(x));
        }

        public void Spawn(UnitInfo unitInfo, HexCell spawnHex = null)
        {
            HexCell spawnPos;
            if (spawnHex == null)
            {
                spawnPos = _hexGrid.spawnPoses.ToList().FirstOrDefault(x => x.isSpawnPos);
                if (spawnPos == null)
                    return;
            }
            else
            {
                spawnPos = spawnHex;
            }

            if (unitInfo.isPlayer)
            {
                var player = new Unit(unitInfo, _chosenWeapon, _hexGrid, _data);
                PlayerControl playerControl = null;

                CameraControl cameraControl =
                    new CameraControl(Camera.main, _data.CameraData);
                _controllers.Add(cameraControl);

                player.OnSpawned += p =>
                {
                    playerControl = new PlayerControl(player, _uiController.PlayerControlView,
                        _uiController.PlayerInventoryView);
                    _controllers.Add(playerControl);
                };
                
                
                player.OnSpawned += unit => _uiController.CheatMenu.SetPlayerNData((Unit)unit, _data);
                player.OnDeath += unit1 => _controllers.Remove(playerControl);
                player.OnDeath += u => playerControl.Dispose();
                player.OnSpawned += unit => cameraControl.InitCameraControl(unit.Instance);
                player.OnDeath += unit => _uiController.CheatMenu.OnPlayerDeath();
                
                player.OnDeath += p => _uiController.AdsMob.ShowCanvas(unitInfo, this);
               
                player.Spawn(spawnPos.coordinates, spawnPos);
                spawnPos.isSpawnPos = false;
                player.BaseView.SetBar(_data.UnitData.PlayerBarCanvas, _data.UnitData.AttackAimCanvas);
                Player = player;
            }
            else
            {
                var enemy = new Unit(unitInfo,
                    _data.WeaponsData.WeaponsList[Random.Range(0, _data.WeaponsData.WeaponsList.Count - 1)], _hexGrid, _data);

                if (unitInfo.isAI)
                {
                    AIAgent agent = new AIAgent(enemy);
                    enemy.OnSpawned += x => _controllers.Add(agent);
                    enemy.OnDeath += x => { _controllers.Remove(agent); };
                }

                enemy.Spawn(spawnPos.coordinates, spawnPos);
                spawnPos.isSpawnPos = false;
                
                enemy.BaseView.SetBar(_data.UnitData.BotBarCanvas, _data.UnitData.AttackAimCanvas);
            }
        }
    }
}