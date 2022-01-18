using System;
using DefaultNamespace;
using HexFiled;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    [CreateAssetMenu(fileName = "CaptureAbility", menuName = "Item/Ability")]
    public class CaptureAbility : Item
    {
        [SerializeField] private GameObject AimCanvas;
        private GameObject _aimInstance;
        
        
        
        public void Invoke(Action action)
        {
            OnItemUsed += action;
            _aimInstance = SpawnHelper.Spawn(AimCanvas, Vector3.zero, Unit.Instance);
        }

        public void Aim(Vector2 direction)
        {
            _aimInstance.transform.LookAt(HexManager.UnitCurrentCell[Unit.Color].cell
                .GetNeighbor(DirectionHelper.VectorToDirection(direction)).transform);
        }

        public void UseAbility()
        {
            Unit.UseItem(this);
           
            OnItemUsed?.Invoke();
        }
    }
}