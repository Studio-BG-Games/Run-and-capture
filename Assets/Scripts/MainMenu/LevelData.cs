using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenu
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        [Serializable]
        public struct Level
        {
            public string sceneName;
            public Sprite levelSprite;
        }
    
        [SerializeField] private List<Level> levels;

        public List<Level> Levels => levels;
    }
}
