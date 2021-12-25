using Chars;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UIData", menuName = "Data/UI Data", order = 0)]
    public class UIData : ScriptableObject
    {
        public PlayerControlView joystickView;
    }
}