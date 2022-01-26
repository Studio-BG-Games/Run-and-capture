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
        private List<ItemInfos> items;

        [SerializeField] private float fromTimeSpawn;
        [SerializeField] private float toTimeSpawn;
        [SerializeField] private List<ItemIcon> icons;

        public List<ItemInfos> ItemInfos => items;

        public List<ItemIcon> Icons => icons;

        public (float from, float to) SpawnTime => (fromTimeSpawn, toTimeSpawn);
    }

    [Serializable]
    public struct ItemInfos
    {
        [SerializeField] private Item item;
        
        
        [SerializeField][Range(0,1)] private float _spawnChance;

        public Item Item => item;
        
        
        public float SpawnChance => _spawnChance;
    }
   
}