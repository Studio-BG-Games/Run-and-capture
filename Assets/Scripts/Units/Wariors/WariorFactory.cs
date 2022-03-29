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

        public delegate UnitBase SpawnWarior(WariorInfo wariorInfo, UnitColor unitColor);
        public UnitBase Spawn(WariorInfo wariorInfo , UnitColor unitColor)
        {
            unitColor = UnitColor.Yellow;
            var spawnPos =
            HexManager.CellByColor[unitColor].Where(x => x != null).ToList()[
              Random.Range(0, HexManager.CellByColor[unitColor].Count - 1)];

            var patrol = new TestInvader(wariorInfo,_data.WeaponsData.WeaponsList[Random.Range(0, _data.WeaponsData.WeaponsList.Count - 1)], _hexGrid,unitColor);

            AIInvader agent = new AIInvader (patrol);
            patrol.OnSpawned += x => _controllers.Add(agent);
            patrol.OnDeath += x => { _controllers.Remove(agent); };

            patrol.Spawn(spawnPos.coordinates, spawnPos);
            spawnPos.isSpawnPos = false;

            patrol.BaseView.SetBar(_data.UnitData.BotBarCanvas, _data.UnitData.AttackAimCanvas);
            return patrol;
        }
    }
}