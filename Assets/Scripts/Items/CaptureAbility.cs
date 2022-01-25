using System;
using System.Collections.Generic;
using DefaultNamespace;
using HexFiled;
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
        


        public void Invoke(Action action)
        {
            OnItemUsed += action;
            if(_aimInstance == null || !_aimInstance.activeSelf)
                _aimInstance = Object.Instantiate(aimCanvas, Unit.Instance.transform);
            else
            {
                _aimInstance.SetActive(true);
            }
        }

        public void Aim(HexDirection direction)
        {
            _aimInstance.transform.LookAt(HexManager.UnitCurrentCell[Unit.Color].cell
                .GetNeighbor(direction).transform);
            _direction = direction;
        }

        private void DoPaint()
        {
            Unit.UseItem(this);
            var cell = HexManager.UnitCurrentCell[Unit.Color].cell.GetNeighbor(_direction);
             cell.PaintHex(Unit.Color);
            bool keepGoing = true;
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
                cell.PaintHex(Unit.Color);
            });
            
            OnItemUsed?.Invoke();
            Unit.UnitView.AnimActionDic[animName] -= DoPaint;
            OnItemUsed = null;
        }

        public void UseAbility()
        {
            var cell = HexManager.UnitCurrentCell[Unit.Color].cell.GetNeighbor(_direction);
            Unit.RotateUnit(new Vector2((cell.transform.position - Unit.Instance.transform.position).normalized.x,
                (cell.transform.position - Unit.Instance.transform.position).normalized.z));
            Unit.Animator.SetTrigger(animName);
            _aimInstance.SetActive(false);
            Unit.UnitView.AnimActionDic[animName] += DoPaint;
        }
    }
}