﻿using System.Collections.Generic;
using AI;
using CamControl;
using Chars;
using DefaultNamespace;
using DefaultNamespace.AI;
using GameUI;
using HexFiled;
using Items;
using Units;
using UnityEngine;
using Weapons;
using Random = UnityEngine.Random;

namespace Controller
{
    internal sealed class GameInit
    {
        public GameInit(Controllers controllers, Data.Data data)
        {
            new AIManager(data.AIData);
            var hexGrid = new HexGrid(data.FieldData);
            new MusicController();
            new VFXController();
            MusicController.Instance.SetMusicData(data.MusicData);
            controllers.Add(hexGrid);
            var paintedController = new PaintedController();

            data.WeaponsData.WeaponsList.ForEach(x => x.SetModifiedDamage(0));

            ItemFabric itemFabric = new ItemFabric(data.ItemsData);
            hexGrid.OnGridLoaded += () => controllers.Add(itemFabric);

            UIController uiController = new UIController(data.UIData);
            uiController.Spawn(); //TODO при паузе Dotween ругается
            Unit player;
            List<Unit> units = new List<Unit>();
            data.UnitData.Units.ForEach(unit =>
            {
                if (unit.isPlayer)
                {
                    var weapon = JsonUtility.FromJson<Weapon>(data.ChosenWeapon);
                    weapon.SetModifiedDamage(0);

                    player = new Unit(unit, weapon, hexGrid);
                    PlayerControl playerControl = new PlayerControl(player, uiController.PlayerControlView,
                        uiController.PlayerInventoryView);
                    controllers.Add(playerControl);
                    CameraControl cameraControl =
                        new CameraControl(Camera.main, data.CameraData);
                    controllers.Add(cameraControl);

                    player.onPlayerSpawned += p => controllers.Add(playerControl);

                    player.OnDeath += unit1 => controllers.Remove(playerControl);

                    player.onPlayerSpawned += cameraControl.InitCameraControl;
                    units.Add(player);

                   // player.OnDeath += uiController.AdsMob.ShowCanvas;
                    player.OnDeath += paintedController.PaintOnDeath;
                }
                else
                {
                    var enemy = new Unit(unit,
                        data.WeaponsData.WeaponsList[Random.Range(0, data.WeaponsData.WeaponsList.Count - 1)], hexGrid);
                    var enemyController = new EnemyController(unit, enemy);
                    controllers.Add(enemyController);
                    units.Add(enemy);
                    AIAgent agent = new AIAgent(unit, enemy);
                    controllers.Add(agent);
                    enemy.OnDeath += x => { controllers.Remove(agent); };
                    enemy.OnDeath += paintedController.PaintOnDeath;
                }
                
            });

            var unitFactory = new UnitFactory(units, hexGrid);

            hexGrid.OnGridLoaded += unitFactory.Spawn;
            
            hexGrid.OnHexPainted += paintedController.SetHexColors;
            hexGrid.OnHexPainted += itemFabric.UpdateCellToOpenList;
            hexGrid.OnHexPainted += paintedController.CheckDeathOrDestroy;
        }
        
    }
}