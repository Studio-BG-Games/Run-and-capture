﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Chars;
using HexFiled;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Data/UnitData", order = 0)]
    public class UnitData : ScriptableObject
    {
        [SerializeField] private List<UnitInfo> _units;
        [SerializeField] private BarCanvas playerBarCanvas;
        [SerializeField] private BarCanvas botBarCanvas;
        [SerializeField] private GameObject attackAimCanvas;
        

       

        public GameObject AttackAimCanvas => attackAimCanvas;

        public List<UnitInfo> Units => _units;

        public BarCanvas PlayerBarCanvas => playerBarCanvas;

        public BarCanvas BotBarCanvas => botBarCanvas;
    }

    [Serializable]
    public struct UnitInfo
    {
        public bool isPlayer;
        public bool isAI;
        public HexCoordinates spawnPos;
        public GameObject unitPrefa;
        public UnitColor color;
        public int manaRegen;
        public int maxMana;
        public int maxHP;
        public int inventoryCapacity;
        public Material InvisibleMaterial;
    }   
}