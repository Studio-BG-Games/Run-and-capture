using System;
using System.Collections.Generic;
using System.IO;
using Items;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData", order = 0)]
    public class ItemsData : ScriptableObject
    {
        [SerializeField]
        private List<ItemInfo> itemInfos;

        [SerializeField] private float fromTimeSpawn;
        [SerializeField] private float toTimeSpawn;

        public List<ItemInfo> ItemInfos => itemInfos;
        public (float from, float to) SpawnTime => (fromTimeSpawn, toTimeSpawn);
    }

    [Serializable]
    public struct ItemInfo
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool isInstantUse;
        [SerializeField] private string type;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject spawnablePrefab;
        [SerializeField] private int[] values;
        [SerializeField][Range(0,1)] private float spawnChance;
        public GameObject Prefab => prefab;
        public string Type => type;

        public Sprite Icon => icon;

        public GameObject SpawnablePrefab => spawnablePrefab;

        public bool IsInstanceUse => isInstantUse;
        public int[] Values => values;
        public float SpawnChance => spawnChance;
    }
}