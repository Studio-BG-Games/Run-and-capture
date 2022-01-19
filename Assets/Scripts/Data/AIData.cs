using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "AIData", menuName = "Data/AIData", order = 0)]
    public class AIData : ScriptableObject
    {
       [SerializeField] private int _distanceToAgr;
       [SerializeField][Range(0,1)] private float _percentToRetreet;
       [SerializeField][Range(0,1)] private float _percentToUseProtectBonus;
       [SerializeField][Range(0,1)] private float _manaPercentToCollectBonus;
       [SerializeField] private int _distaceToCollectBonus;

       public int DistanceToAgr => _distanceToAgr;

       public float PercentToUseProtectBonus => _percentToUseProtectBonus;

       public float PercentToRetreet => _percentToRetreet;

       public float ManaPercentToCollectBonus => _manaPercentToCollectBonus;

       public int DistaceToCollectBonus => _distaceToCollectBonus;
    }
}