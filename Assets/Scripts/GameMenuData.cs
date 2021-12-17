using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "GameMenuData", menuName = "Data/GameMenuData", order = 0)]
    public class GameMenuData : ScriptableObject
    {
        public bool isMusicAllowed;
        public bool isSFXAllowed;
    }
}