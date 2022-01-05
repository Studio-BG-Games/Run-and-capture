using System.Collections.Generic;
using Units;

namespace Chars
{
    public class UnitFactory
    {
        private List<Unit> _units;

        public UnitFactory(List<Unit> units)
        {
            _units = units;
        }

        public void Spawn()
        {
            _units.ForEach(x => x.Spawn());
        }
    }
}