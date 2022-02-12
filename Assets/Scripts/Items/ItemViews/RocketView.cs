using System;
using System.Collections.Generic;
using System.Linq;
using AI;
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

        public void SetUp(Unit unit)
        {
            _unit = unit.Instance;
            _color = unit.Color;
        }

        private void GetNearestUnit()
        {
            Rockets[_color].SetActive(true);
            listUnits = new List<GameObject>();
            listUnits.AddRange(HexManager.UnitCurrentCell.Where(x => x.Key != _color).ToList().Select(x => x.Value.unit.Instance));
            listUnits.Sort((x, y) =>
                Vector3.Distance(x.transform.position, _unit.transform.position).CompareTo(
                    Vector3.Distance(y.transform.position, _unit.transform.position)));
        }

        private void Update()
        {
            if (_unit != null)
            {
                GetNearestUnit();
                transform.DOKill();
                transform.LookAt(listUnits.First().transform);
                transform.DOMove(listUnits.First().transform.position, Vector3.Distance(listUnits.First().transform.position, _unit.transform.position)*0.2f).SetEase(Ease.Linear);
            }
        }
    }
}