using UnityEngine;

namespace Chars
{
    public class PlayerControlView : MonoBehaviour
    {
        [SerializeField] private FloatingJoystick moveJoystick;
        [SerializeField] private FloatingJoystick attackJoystick;
        [SerializeField] private FloatingJoystick placeJoystick;
        

        public FloatingJoystick MoveJoystick => moveJoystick;
        public FloatingJoystick AttackJoystick => attackJoystick;

        public FloatingJoystick PlaceJoystick => placeJoystick;
    }
}
