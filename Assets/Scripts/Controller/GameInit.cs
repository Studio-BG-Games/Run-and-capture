using HexFiled;
using UnityEngine;

namespace Controller
{
    internal sealed class GameInit
    {
        public GameInit(Controllers controllers, Data.Data data)
        {
           var hexGrid = new HexGrid(data.Field);
           controllers.Add(hexGrid);
           hexGrid.OnHexPainted += DoSomething;
        }

        private void DoSomething()
        {
            Debug.Log("Painted!" );
        }
    }
}