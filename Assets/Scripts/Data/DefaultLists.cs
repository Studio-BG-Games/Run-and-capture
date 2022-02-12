using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Defaults", menuName = "Data/Defaults", order = 0)]
    public class DefaultLists : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<BuildingKeys, GameObject> buildings;

        public Dictionary<BuildingKeys, GameObject> Buildings => buildings;
    }

    public enum BuildingKeys
    {
        None,
        Tree,
        Bomb
    }
}