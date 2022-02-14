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
        private GameObject _aimInstance;
        private HexDirection _direction;
      


        public void Invoke(Action<Unit> action, Unit unit)
        {
            OnItemUsed ??= action;

            if(_aimInstance == null)
                _aimInstance = Object.Instantiate(aimCanvas, unit.Instance.transform);
            _aimInstance.SetActive(false);
        }

        public void Aim(HexDirection direction, Unit unit)
        {
            if(_aimInstance == null)
                _aimInstance = Object.Instantiate(aimCanvas, unit.Instance.transform);
            _aimInstance.SetActive(true);
            var cell = HexManager.UnitCurrentCell[unit.Color].cell
                .GetNeighbor(direction);
            if (cell == null)
            {
                return;
            }
            _aimInstance.transform.LookAt(cell.transform);
            _direction = direction;
        }

        public void DeAim()
        {
            _aimInstance.SetActive(false);
        }
        
        private void DoPaint(Unit unit)
        {
            unit.UseItem(this);
            HexManager.UnitCurrentCell[unit.Color].cell.PaintHex(unit.Color);
            var cell = HexManager.UnitCurrentCell[unit.Color].cell.GetNeighbor(_direction);
            cell.PaintHex(unit.Color);
            bool keepGoing = true;
            var moveDir = _direction;
            itterationMove.ForEach(dir =>
            {
                if (!keepGoing) return;
                _direction = dir switch
                {
                    Angls.FORWARD => _direction,
                    Angls.PLUS60 => _direction.PlusSixtyDeg(),
                    Angls.MINUS60 => _direction.MinusSixtyDeg(),
                    Angls.PLUS120 => _direction.Plus120Deg(),
                    Angls.MINUS120 => _direction.Minus120Deg(),
                    Angls.BACK => _direction.Back(),
                    _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
                };
                if (cell.GetNeighbor(_direction) == null)
                {
                    keepGoing = false;
                    return;
                }

                cell = cell.GetNeighbor(_direction);
                cell.PaintHex(unit.Color);
            });
            
            OnItemUsed?.Invoke(unit);
            
            
            unit.UnitView.AnimActionDic[animName] -= DoPaint;
            OnItemUsed = null;
        }

        public void UseAbility(Unit unit)
        {
            
            var cell = HexManager.UnitCurrentCell[unit.Color].cell.GetNeighbor(_direction);
            if (cell == null)
            {
                DeAim();
                return;
            }
            unit.RotateUnit(new Vector2((cell.transform.position - unit.Instance.transform.position).normalized.x,
                (cell.transform.position - unit.Instance.transform.position).normalized.z));
            unit.Animator.SetTrigger(animName);
            _aimInstance.SetActive(false);
            unit.SetCell(cell);
            unit.UnitView.AnimActionDic[animName] += DoPaint;
        }
    }
}