using HexFiled;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData", order = 0)]
    public class PlayerData : ScriptableObject
    {
        public HexCoordinates SpawnPos;
        public GameObject PlayerPrefab;
        public FloatingJoystick Joystick;
    }
}