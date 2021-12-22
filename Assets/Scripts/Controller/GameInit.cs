using System.Collections.Generic;
using CamControl;
using Chars;
using Data;
using HexFiled;
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

            Player player = new Player(data.PlayerData, data.WeaponsData.WeaponsList[0], hexGrid);
            List<IUnit> units = new List<IUnit> { player };
            data.EnemyData.Enemies.ForEach(enemyInfo =>
            {
                var enemy = new Enemy(enemyInfo, hexGrid);
                var enemyController = new EnemyController(enemyInfo, enemy);
                controllers.Add(enemyController);
                units.Add(enemy);
            });
            
            var unitFactory = new UnitFactory(units);
            hexGrid.OnGridLoaded += unitFactory.Spawn;

            PlayerControl playerControl = new PlayerControl(player, data.PlayerData);
            controllers.Add(playerControl);

            CameraControl cameraControl =
                new CameraControl(Camera.main, data.CameraData);
            controllers.Add(cameraControl);
            player.onPlayerSpawned += cameraControl.InitCameraControl;
        }
        private void DoSomething(HexCell cell)
        {
            Debug.Log("Painted! " + cell.coordinates );
        }
    }
}