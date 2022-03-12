using Controller;
using Data;
using DefaultNamespace.AI;
using GameUI;
using HexFiled;
using System.Linq;
using Units.Wariors.AbstractsBase;
using UnityEngine;
using Weapons;

namespace Units.Wariors
{
    public class WariorFactory
    {
        private readonly HexGrid _hexGrid;
        private readonly Controllers _controllers;
        private readonly Data.Data _data;

        public WariorFactory(HexGrid grid, Controllers controllers, Data.Data data)
        {
            _hexGrid = grid;
            _data = data;
            _controllers = controllers;
        }


        public void Spawn(WariorInfo wariorInfo , UnitColor unitColor)
        {
            var spawnPos= HexManager.CellByColor[unitColor].Where(x => x != null).ToList()[
                    Random.Range(0, HexManager.CellByColor[unitColor].Count - 1)];

            var patrol = new Holem(wariorInfo,_data.WeaponsData.WeaponsList[Random.Range(0, _data.WeaponsData.WeaponsList.Count - 1)], _hexGrid,unitColor);

            AIAgent agent = new AIAgent(patrol);
            patrol.OnSpawned += x => _controllers.Add(agent);
            patrol.OnDeath += x => { _controllers.Remove(agent); };

            patrol.Spawn(spawnPos.coordinates, spawnPos);
            spawnPos.isSpawnPos = false;

            patrol.UnitView.SetBar(_data.UnitData.BotBarCanvas, _data.UnitData.AttackAimCanvas);
        }
    }
}