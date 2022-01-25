using System.Collections;
using System.Collections.Generic;
using Data;
using HexFiled;
using Units;
using UnityEngine;

public class CheatMenu : MonoBehaviour
{
    [SerializeField] private UnitData _data;
    private Unit _player;
    void Start()
    {
         _player = HexManager.UnitCurrentCell[_data.Units.Find(x => x.isPlayer).color].unit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
