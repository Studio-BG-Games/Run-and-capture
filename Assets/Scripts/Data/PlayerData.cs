using Chars;
using HexFiled;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData", order = 0)]
    public class PlayerData : ScriptableObject
    {
        public HexCoordinates spawnPos;
        public GameObject playerPrefab;
        public PlayerControlView joystickView;
    }
}