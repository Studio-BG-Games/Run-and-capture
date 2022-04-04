﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CamControl;
using Controller;
using Data;
using DefaultNamespace;
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
        public Units.Unit.Unit Player { get; private set; }

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
            _uiController.CheatMenu.SetPlayerNData(_data);
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
                var player = new Units.Unit.Unit(unitInfo, _chosenWeapon, _hexGrid);
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
                var enemy = new Units.Unit.Unit(unitInfo,
                    _data.WeaponsData.WeaponsList[Random.Range(0, _data.WeaponsData.WeaponsList.Count - 1)], _hexGrid);


                enemy.OnDeath += unit => RandomSpawn(unitInfo);

                if (unitInfo.isAI)
                {
                    AIWarior agent = new AIWarior(enemy);
                    enemy.OnSpawned += x => _controllers.Add(agent);
                    enemy.OnDeath += x => { _controllers.Remove(agent); };
                }

                enemy.OnDeath += x => _uiController.CheatMenu.OnEnemyDeath();
                
                enemy.Spawn(spawnPos.coordinates, spawnPos);
                spawnPos.isSpawnPos = false;
                enemy.BaseView.SetBar(_data.UnitData.BotBarCanvas, _data.UnitData.AttackAimCanvas);
            }
        }

        private void RandomSpawn(UnitInfo info)
        {
            TimerHelper.Instance.StartTimer(() =>
            {
                var cellToSpawn = HexManager.CellByColor[UnitColor.Grey].Where(cell => cell != null &&
                    cell.GetListNeighbours().TrueForAll(neighbour => neighbour == null || neighbour.Color == UnitColor.Grey)).ToList();
                if (cellToSpawn.Count == 0)
                {
                    RandomSpawn(info);
                    return;
                }
                
                Spawn(info, cellToSpawn[Random.Range(0, cellToSpawn.Count - 1)]);
                _uiController.CheatMenu.OnEnemyDeath();
            }, 5f);
        }
    }
}