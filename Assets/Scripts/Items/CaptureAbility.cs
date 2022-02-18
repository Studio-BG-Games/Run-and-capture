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
            container.Direction = direction;
        }

        
        
        private void DoPaint(ItemContainer container)
        {
            container.Unit.UseItem(this);
            HexManager.UnitCurrentCell[container.Unit.Color].cell.PaintHex(container.Unit.Color);
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell.GetNeighbor(container.Direction);
            container.OnItemUsed?.Invoke();

            container.Unit.UnitView.AnimActionDic[animName] = null;
            
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
                container.Direction = dir switch
                {
                    Angls.FORWARD => container.Direction,
                    Angls.PLUS60 => container.Direction.PlusSixtyDeg(),
                    Angls.MINUS60 => container.Direction.MinusSixtyDeg(),
                    Angls.PLUS120 => container.Direction.Plus120Deg(),
                    Angls.MINUS120 => container.Direction.Minus120Deg(),
                    Angls.BACK => container.Direction.Back(),
                    _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
                };
                if (cell.GetNeighbor(container.Direction) == null)
                {
                    keepGoing = false;
                    return;
                }

                cell = cell.GetNeighbor(container.Direction);
                cell.PaintHex(container.Unit.Color);
            });
            
           
        }

        public void UseAbility(ItemContainer container)
        {
            
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell.GetNeighbor(container.Direction);
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
            container.Unit.UnitView.AnimActionDic[animName] += () => DoPaint(container);
        }
    }
}