using System;
using System.Collections.Generic;
using Chars;
using HexFiled;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Data/UnitData", order = 0)]
    public class UnitData : ScriptableObject
    {
        [SerializeField] private List<UnitInfo> _units;

        public List<UnitInfo> Units => _units;
    }

    [Serializable]
    public struct UnitInfo
    {
        public bool isPlayer;
        public HexCoordinates spawnPos;
        public GameObject unitPrefa;
        public UnitColor color;
        public int manaRegen;
        public int maxMana;
        public int maxHP;
    }
}