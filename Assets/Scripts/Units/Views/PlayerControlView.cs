using UnityEngine;

namespace Chars
{
    public class PlayerControlView : MonoBehaviour
    {
        [SerializeField] private FloatingJoystick moveJoystick;
        [SerializeField] private FloatingJoystick attackJoystick;
        

        public FloatingJoystick MoveJoystick => moveJoystick;
        public FloatingJoystick AttackJoystick => attackJoystick;
        
        
        
    }
}
