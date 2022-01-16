using UnityEngine;

namespace Chars
{
    public class PlayerControlView : MonoBehaviour
    {
        [SerializeField] private Joystick moveJoystick;
        [SerializeField] private Joystick attackJoystick;
        [SerializeField] private Joystick placeJoystick;
        

        public Joystick MoveJoystick => moveJoystick;
        public Joystick AttackJoystick => attackJoystick;

        public Joystick PlaceJoystick => placeJoystick;
    }
}
