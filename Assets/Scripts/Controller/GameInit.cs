using AI;
using Chars;
using DefaultNamespace;
using GameUI;
using HexFiled;
using Items;
using Units.Wariors;
using UnityEngine;


namespace Controller
{
    internal sealed class GameInit
    {
        public GameInit(Controllers controllers, Data.Data data)
        {
            new Unit(data.AIData);
            new Warior(data.AIData);
            var hexGrid = new HexGrid(data.FieldData);
            new MusicController();
            new VFXController();
            MusicController.Instance.SetMusicData(data.MusicData);
            Time.timeScale = 1f;
            var paintedController = new PaintedController();
            

            ItemFabric itemFabric = new ItemFabric(data.ItemsData);
            hexGrid.OnGridLoaded += () => controllers.Add(itemFabric);

            UIController uiController = new UIController(data.UIData);
            uiController.Spawn(); //TODO при паузе Dotween ругается
            

            var unitFactory = new UnitFactory(hexGrid, data, uiController, paintedController, controllers);
            var wariorFactory = new WariorFactory(hexGrid, controllers, data);

            hexGrid.OnGridLoaded += () => unitFactory.SpawnList(data.UnitData.Units);

            hexGrid.OnHexPainted += paintedController.SetHexColors;

            hexGrid.SpawnField();
            unitFactory.Player.OnShoot += wariorFactory.Spawn;
        }
        
    }
}