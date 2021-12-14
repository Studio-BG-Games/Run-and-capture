using Runtime.Controller;

namespace Chars
{
    public class PlayerControl : IExecute
    {
        private Player _player;

        public PlayerControl(Player player)
        {
            _player = player;
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}