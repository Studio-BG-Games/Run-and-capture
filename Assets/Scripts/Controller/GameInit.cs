using System.Collections.Generic;
using CamControl;
using Chars;
using Data;
using HexFiled;
using Units;
using UnityEngine;

namespace Controller
{
    internal sealed class GameInit
    {
        public GameInit(Controllers controllers, Data.Data data)
        {
            var hexGrid = new HexGrid(data.FieldData);
            controllers.Add(hexGrid);
            hexGrid.OnHexPainted += DoSomething;
            Unit player;
            
            List<Unit> units = new List<Unit>();
            data.UnitData.Units.ForEach(unit =>
            {
                if (unit.isPlayer)
                {
                    player = new Unit(unit, data.WeaponsData.WeaponsList[0], hexGrid);
                    PlayerControl playerControl = new PlayerControl(player, data.UIData);
                    controllers.Add(playerControl);
                    CameraControl cameraControl =
                        new CameraControl(Camera.main, data.CameraData);
                    controllers.Add(cameraControl);
                    player.onPlayerSpawned += cameraControl.InitCameraControl;
                    units.Add(player);
                }
                else
                {
                    var enemy = new Unit(unit,data.WeaponsData.WeaponsList[0], hexGrid);
                    var enemyController = new EnemyController(unit, enemy);
                    controllers.Add(enemyController);
                    units.Add(enemy);
                }
                
            });
            
            var unitFactory = new UnitFactory(units);
            hexGrid.OnGridLoaded += unitFactory.Spawn;
            
            
        }
        private void DoSomething(HexCell cell)
        {
            Debug.Log("Painted! " + cell.coordinates );
        }
    }
}