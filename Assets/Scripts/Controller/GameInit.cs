using System.Collections.Generic;
using Chars;
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

            Player player = new Player(data.PlayerData, hexGrid);
            
            List<IUnit> units = new List<IUnit> { player };
            
            var unitFactory = new UnitFactory(units);
            hexGrid.OnGridLoaded += unitFactory.Spawn;
            
        }

        private void DoSomething(HexCell cell)
        {
            Debug.Log("Painted! " + cell.coordinates );
        }
    }
}