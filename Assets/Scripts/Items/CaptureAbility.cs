using System;
using System.Collections.Generic;
using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Items
{
    enum Angls
    {
        FORWARD,
        PLUS60,
        MINUS60,
        PLUS120,
        MINUS120,
        BACK
    }

    [CreateAssetMenu(fileName = "CaptureAbility", menuName = "Item/Ability")]
    public class CaptureAbility : Item
    {
        [SerializeField] private GameObject aimCanvas;
        [SerializeField] private List<Angls> itterationMove;
        [SerializeField] private string animName;


        public override void Invoke(ItemContainer container)
        {
           

            if (container.AimInstance == null)
            {
                container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
            }
            container.AimInstance.SetActive(false);
        }
        

        public void Aim(HexDirection direction, ItemContainer container)
        {
            if (container.AimInstance == null)
            {
                container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
            }
            container.AimInstance.SetActive(true);
            
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell
                .GetNeighbor(direction);
            if (cell == null)
            {
                return;
            }
            container.AimInstance.transform.LookAt(cell.transform);
            container.HexDirection = direction;
        }

        
        
        private void DoPaint(ItemContainer container)
        {
            container.Unit.UseItem(this);
            HexManager.UnitCurrentCell[container.Unit.Color].cell.PaintHex(container.Unit.Color);
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell.GetNeighbor(container.HexDirection);
            container.OnItemUsed?.Invoke();

            container.Unit.BaseView.AnimActionDic[animName] = null;
            
            container.OnItemUsed = null;
            if (cell == null)
            {
                return;
            }
            cell.PaintHex(container.Unit.Color);
            bool keepGoing = true;
            
            
            itterationMove.ForEach(dir =>
            {
                if (!keepGoing) return;
                container.HexDirection = dir switch
                {
                    Angls.FORWARD => container.HexDirection,
                    Angls.PLUS60 => container.HexDirection.PlusSixtyDeg(),
                    Angls.MINUS60 => container.HexDirection.MinusSixtyDeg(),
                    Angls.PLUS120 => container.HexDirection.Plus120Deg(),
                    Angls.MINUS120 => container.HexDirection.Minus120Deg(),
                    Angls.BACK => container.HexDirection.Back(),
                    _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
                };
                if (cell.GetNeighbor(container.HexDirection) == null)
                {
                    keepGoing = false;
                    return;
                }

                cell = cell.GetNeighbor(container.HexDirection);
                cell.PaintHex(container.Unit.Color);
            });
            
           
        }

        public void UseAbility(ItemContainer container)
        {
            
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell.GetNeighbor(container.HexDirection);
            if (cell == null)
            {
                container.DeAim();
                return;
            }
            container.Unit.RotateUnit(new Vector2((cell.transform.position - container.Unit.Instance.transform.position).normalized.x,
                (cell.transform.position - container.Unit.Instance.transform.position).normalized.z));
            container.Unit.Animator.SetTrigger(animName);
            container.DeAim();
            container.Unit.SetCell(cell);
            container.Unit.BaseView.AnimActionDic[animName] += () => DoPaint(container);
        }
    }
}