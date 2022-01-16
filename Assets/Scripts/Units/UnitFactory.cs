using System.Collections.Generic;
using HexFiled;
using Units;

namespace Chars
{
    public class UnitFactory
    {
        private List<Unit> _units;
     

        public UnitFactory(List<Unit> units, HexGrid grid)
        {
            _units = units;
        
        }

        public void Spawn()
        {
            _units.ForEach(x => x.Spawn(x.Data.spawnPos));
        }
    }
}