using System.Collections.Generic;
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
        private readonly PaintedController _paintedController;

        public UnitFactory(HexGrid grid, Data.Data data, UIController uiController, PaintedController paintedController,
            Controllers controllers)
        {
            _hexGrid = grid;
            _data = data;
            _chosenWeapon = data.ChosenWeapon;
            _uiController = uiController;
            _paintedController = paintedController;
            _controllers = controllers;
        }

        public void SpawnList(List<UnitInfo> units)
        {
            units.ForEach(Spawn);
        }

        public void Spawn(UnitInfo unitInfo)
        {
            if (unitInfo.isPlayer)
            {
                var player = new Unit(unitInfo, _chosenWeapon, _hexGrid);
                PlayerControl playerControl = null;

                CameraControl cameraControl =
                    new CameraControl(Camera.main, _data.CameraData);
                _controllers.Add(cameraControl);

                player.OnPlayerSpawned += p =>
                {
                    playerControl = new PlayerControl(player, _uiController.PlayerControlView,
                        _uiController.PlayerInventoryView);
                    _controllers.Add(playerControl);
                };

                _uiController.CheatMenu.SetPlayerNData(player, _data);
                
                player.OnDeath += unit1 => _controllers.Remove(playerControl);
                player.OnDeath += u => playerControl.Dispose();
                player.OnPlayerSpawned += cameraControl.InitCameraControl;

                player.OnDeath += p => _uiController.AdsMob.ShowCanvas(unitInfo, this);
                player.OnDeath += _paintedController.PaintOnDeath;
                player.Spawn(unitInfo.spawnPos);
            }
            else
            {
                var enemy = new Unit(unitInfo,
                    _data.WeaponsData.WeaponsList[Random.Range(0, _data.WeaponsData.WeaponsList.Count - 1)], _hexGrid);


                AIAgent agent = new AIAgent(unitInfo, enemy);
                enemy.OnPlayerSpawned += x => _controllers.Add(agent);
                enemy.OnDeath += x => { _controllers.Remove(agent); };
                enemy.OnDeath += _paintedController.PaintOnDeath;
                enemy.Spawn(unitInfo.spawnPos);
            }
        }
    }
}