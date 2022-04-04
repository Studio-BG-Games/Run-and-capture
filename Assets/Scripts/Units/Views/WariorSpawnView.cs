using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Units.Wariors;
using System;
using HexFiled;
using System.Linq;
using Units.Wariors.AbstractsBase;
using Random = UnityEngine.Random;
using DefaultNamespace.AI;

public class WariorSpawnView : MonoBehaviour
{
    [SerializeField] private Button wariorPanelOpen;
    [Space]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closePanel;
    [SerializeField] private Button warior1;
    [SerializeField] private Button warior2;
    [SerializeField] private Button warior3;

    [HideInInspector] public Button Warior1 => warior1;
    [HideInInspector] public Button Warior2 => warior2;
    [HideInInspector] public Button Warior3 => warior3;

    private void Start()
    {
        spawnAttacked attacked = new spawnAttacked(SpawnAttacked);
        spawnPatrol patrol = new spawnPatrol(SpawnPatrol);
        spawnInvader invader = new spawnInvader(SpawnInvader);
        War1 += SpawnInvader;
        War2 += patrol.Invoke;
        War3 += attacked.Invoke;
        closePanel.onClick.AddListener(OpenClosePanel(false));
        wariorPanelOpen.onClick.AddListener(OpenClosePanel(true));
        warior1.onClick.AddListener(War1);
        warior2.onClick.AddListener(War2);
        warior3.onClick.AddListener(War3);
    }
    private UnityAction OpenClosePanel(bool state) => () => panel.SetActive(state);
    public UnityAction War1;
    public UnityAction War2;
    public UnityAction War3;

    public delegate void spawnInvader();
    public delegate void spawnPatrol();
    public delegate void spawnAttacked();

    public void SpawnInvader()
    {
        Debug.Log("заспавнил врага1");
        var unitColor = UnitColor.Yellow;
        var spawnPos =
        HexManager.CellByColor[unitColor].Where(x => x != null).ToList()[
        Random.Range(0, HexManager.CellByColor[unitColor].Count - 1)];

        var patrol = new TestInvader(WariorFactory._wariorData.Wariors, WariorFactory._data.WeaponsData.WeaponsList[Random.Range(0, WariorFactory._data.WeaponsData.WeaponsList.Count - 1)], WariorFactory._hexGrid, unitColor);

        AIInvader agent = new AIInvader(patrol);
        patrol.OnSpawned += x => WariorFactory._controllers.Add(agent);
        patrol.OnDeath += x => { WariorFactory._controllers.Remove(agent); };

        patrol.Spawn(spawnPos.coordinates, spawnPos);
        spawnPos.isSpawnPos = false;

        patrol.BaseView.SetBar(WariorFactory._data.UnitData.BotBarCanvas, WariorFactory._data.UnitData.AttackAimCanvas);
    }
    public void SpawnPatrol()
    {
        var unitColor = UnitColor.Yellow;
        var spawnPos =
        HexManager.CellByColor[unitColor].Where(x => x != null).ToList()[
        Random.Range(0, HexManager.CellByColor[unitColor].Count - 1)];

        var patrol = new Holem(WariorFactory._wariorData.Wariors, WariorFactory._data.WeaponsData.WeaponsList[Random.Range(0, WariorFactory._data.WeaponsData.WeaponsList.Count - 1)], WariorFactory._hexGrid, unitColor);

        AIWarior agent = new AIWarior(patrol);
        patrol.OnSpawned += x => WariorFactory._controllers.Add(agent);
        patrol.OnDeath += x => { WariorFactory._controllers.Remove(agent); };

        patrol.Spawn(spawnPos.coordinates, spawnPos);
        spawnPos.isSpawnPos = false;

        patrol.BaseView.SetBar(WariorFactory._data.UnitData.BotBarCanvas, WariorFactory._data.UnitData.AttackAimCanvas);
        Debug.Log("заспавнил врага2");
    }
    public void SpawnAttacked()
    {
        var unitColor = UnitColor.Yellow;
        var spawnPos =
        HexManager.CellByColor[unitColor].Where(x => x != null).ToList()[
        Random.Range(0, HexManager.CellByColor[unitColor].Count - 1)];

        var patrol = new AssailantTest(WariorFactory._wariorData.Wariors, WariorFactory._data.WeaponsData.WeaponsList[Random.Range(0, WariorFactory._data.WeaponsData.WeaponsList.Count - 1)], WariorFactory._hexGrid, unitColor);

        AIWarior agent = new AIWarior(patrol);
        patrol.OnSpawned += x => WariorFactory._controllers.Add(agent);
        patrol.OnDeath += x => { WariorFactory._controllers.Remove(agent); };

        patrol.Spawn(spawnPos.coordinates, spawnPos);
        spawnPos.isSpawnPos = false;

        patrol.BaseView.SetBar(WariorFactory._data.UnitData.BotBarCanvas, WariorFactory._data.UnitData.AttackAimCanvas);
        Debug.Log("заспавнил врага3");

    }
}
