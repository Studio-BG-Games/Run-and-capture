using System.Collections.Generic;
using DefaultNamespace.AI;

namespace AI
{
    public class AIManager
    {
        private List<AIAgent> _agents;
        
        // private void SetBehaviour(BotState state)
        // {
        //     //leftInput = Vector2.zero;
        //     switch (state)
        //     {
        //         case BotState.Patrol:
        //             MoveAlongPath();
        //             break;
        //         case BotState.Agressive:
        //             MoveToEnemy(_currentEnemy);
        //             break;
        //         case BotState.Attack:
        //             AttackEnemy(_currentEnemy);
        //             break;
        //         case BotState.CollectingBonus:
        //             MoveToBonus(_currentTargetTile);
        //             break;
        //         case BotState.ProtectBonusUsage:
        //             PlaceProtectBonus(_playerState.currentTile);
        //             break;
        //     }
        // }
        
    }
    
    
    
    public enum BotState
    {
        Patrol,
        Agressive,
        Attack,
        CollectingBonus,
        AttackBonusUsage,
        ProtectBonusUsage,
        Dead
    }
}