using System;
using System.Collections.Generic;
using HexFiled;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData", order = 0)]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private List<EnemyInfo> _enemies;
        public List<EnemyInfo> Enemies => _enemies;
    }

    [Serializable]
    public struct EnemyInfo
    {
        public HexCoordinates spawnPos;
        public GameObject playerPrefab;
        public UnitColor color;
    }
}