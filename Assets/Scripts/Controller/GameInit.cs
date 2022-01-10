using System;
using System.Collections.Generic;
using System.Timers;
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
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Controller
{
    internal sealed class GameInit
    {
        public GameInit(Controllers controllers, Data.Data data)
        {

            AIManager aiManager = new AIManager();
            controllers.Add(aiManager);
            new GameObject("Timer").AddComponent<TimerHelper>();
            
            var hexGrid = new HexGrid(data.FieldData);
            new MusicController();
            MusicController.Instance.SetMusicData(data.MusicData);
            controllers.Add(hexGrid);
            
            data.WeaponsData.WeaponsList.ForEach(x => x.SetModifiedDamage(0));
            
            ItemFabric itemFabric = new ItemFabric(data.ItemsData, SetUpItems());
            controllers.Add(itemFabric);

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
                    player.onPlayerSpawned += cameraControl.InitCameraControl;
                    player.onPlayerSpawned += MusicController.Instance.AddAudioListener;
                    units.Add(player);

                    player.OnDeath += uiController.AdsMob.ShowCanvas;
                }
                else
                {
                    var enemy = new Unit(unit,
                        data.WeaponsData.WeaponsList[Random.Range(0, data.WeaponsData.WeaponsList.Count - 1)], hexGrid);
                    var enemyController = new EnemyController(unit, enemy);
                    controllers.Add(enemyController);
                    units.Add(enemy);
                    AIAgent agent = new AIAgent(unit, enemy, aiManager);
                    controllers.Add(agent);
                }
            });

            var unitFactory = new UnitFactory(units, hexGrid);

            hexGrid.OnGridLoaded += unitFactory.Spawn;

            var paintedController = new PaintedController();

            hexGrid.OnHexPainted += paintedController.SetHexColors;

            hexGrid.OnHexPainted += itemFabric.UpdateCellToOpenList;
            hexGrid.OnHexPainted += paintedController.CheckDeath;
        }

        private List<Type> SetUpItems()
        {
            return new List<Type>() { typeof(Tower), typeof(AttackBonus), typeof(DefenceBonus) };
        }
    }
}