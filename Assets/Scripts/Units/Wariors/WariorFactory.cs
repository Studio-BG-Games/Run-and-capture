using Controller;
using Data;
using DefaultNamespace.AI;
using GameUI;
using HexFiled;
using System.Linq;
using Units.Wariors.AbstractsBase;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

namespace Units.Wariors
{
    public class WariorFactory
    {
        public static HexGrid _hexGrid;
        public static Controllers _controllers;
        public static Data.Data _data;
        public static UIData _uiData;
        public static WariorsData _wariorData;

        public WariorFactory(HexGrid grid, Controllers controllers, Data.Data data)
        {
            _hexGrid = grid;
            _data = data;
            _controllers = controllers;
            _uiData = data.UIData;
            _wariorData = data.WariorsData;
        }

    }
}