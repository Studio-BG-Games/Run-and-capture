using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Units;
using UnityEngine;

namespace Items.ItemViews
{
    public class RocketView : SerializedMonoBehaviour, ISetUp
    {
        [OdinSerialize] private Dictionary<UnitColor, GameObject> Rockets;
        private List<GameObject> listUnits;
        [SerializeField] private GameObject _unit;
        [SerializeField] private UnitColor _color;
        [SerializeField] private float speed;

        public void SetUp(Unit unit)
        {
            _unit = unit.Instance;
            _color = unit.Color;
            Rockets[_color].SetActive(true);
            GetNearestUnit();
            MoveToTarget();
           
        }

        private void MoveToTarget()
        {
            transform.DOKill();
            transform.LookAt(listUnits.First().transform);
            transform.DOMove(listUnits.First().transform.position,
                speed * Vector3.Distance(transform.position, listUnits.First().transform.position)).OnUpdate(
                MoveToTarget);
        }
        private void GetNearestUnit()
        {
            listUnits = new List<GameObject>();
            listUnits.AddRange(HexManager.UnitCurrentCell.Where(x => x.Key != _color).ToList()
                .Select(x => x.Value.unit.Instance));
            listUnits.Sort((x, y) =>
                Vector3.Distance(x.transform.position, _unit.transform.position).CompareTo(
                    Vector3.Distance(y.transform.position, _unit.transform.position)));
        }

       
    }
}