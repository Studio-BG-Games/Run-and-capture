using System.Collections.Generic;
using CamControl;
using Chars;
using Data;
using DefaultNamespace;
using GameUI;
using HexFiled;
using Units;
using UnityEngine;
using Weapons;

namespace Controller
{
    internal sealed class GameInit
    {
        public GameInit(Controllers controllers, Data.Data data)
        {
            var hexGrid = new HexGrid(data.FieldData);
            new MusicController();
            MusicController.Instance.SetMusicData(data.MusicData);
            controllers.Add(hexGrid);
            
            UIController uiController = new UIController(data.UIData);
            uiController.Spawn(); //TODO при паузе 

            Unit player;
            List<Unit> units = new List<Unit>();
            data.UnitData.Units.ForEach(unit =>
            {
                if (unit.isPlayer)
                {
                    player = new Unit(unit, JsonUtility.FromJson<Weapon>(data.ChosenWeapon), hexGrid);
                    PlayerControl playerControl = new PlayerControl(player, uiController.PlayerControlView);
                    controllers.Add(playerControl);
                    CameraControl cameraControl =
                        new CameraControl(Camera.main, data.CameraData);
                    controllers.Add(cameraControl);
                    player.onPlayerSpawned += cameraControl.InitCameraControl;
                    player.onPlayerSpawned += MusicController.Instance.AddAudioListener;
                    units.Add(player);
                }
                else
                {
                    var enemy = new Unit(unit, data.WeaponsData.WeaponsList[Random.Range(0,data.WeaponsData.WeaponsList.Count - 1)], hexGrid);
                    var enemyController = new EnemyController(unit, enemy);
                    controllers.Add(enemyController);
                    units.Add(enemy);
                }
            });

            var unitFactory = new UnitFactory(units);

            hexGrid.OnGridLoaded += unitFactory.Spawn;

            var paintedController = new PaintedController();

            hexGrid.OnHexPainted += paintedController.SetHexColors;

        }
        
        
    }
}