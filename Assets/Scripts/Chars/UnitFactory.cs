using System.Collections.Generic;

namespace Chars
{
    public class UnitFactory
    {
        private List<IUnit> _units;

        public UnitFactory(List<IUnit> units)
        {
            _units = units;
        }

        public void Spawn()
        {
            _units.ForEach(x => x.Spawn());
        }
    }
}