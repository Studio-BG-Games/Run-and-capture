using System;
using System.Collections.Generic;
using System.Linq;
using HexFiled;
using Items;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
    [SerializeField] private Button showButton;
    [SerializeField] private GameObject scrollRect;
    [SerializeField] private GameObject grid;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private GameObject gridPrefab;
    private Unit _player;
    private Data.Data _data;
    private GameObject _itemsPrefab;
    private List<GameObject> _buttons;

    public void SetPlayerNData(Unit player, Data.Data data)
    {
        _buttons = new List<GameObject>();
        _player = player;
        _itemsPrefab = new GameObject("CheatedItems");
        
        showButton.onClick.AddListener(() => scrollRect.SetActive(!scrollRect.activeSelf));
        _data = data;
        AddAllButtons();
        scrollRect.SetActive(false);
        
    }

    private void AddAllButtons()
    {
        
        var itemGridGo = Instantiate(gridPrefab, grid.transform);
        _buttons.Add(itemGridGo);
        var itemGrid = itemGridGo.GetComponentInChildren<GridLayoutGroup>();
        itemGridGo.GetComponentInChildren<TMP_Text>().text = "Items";
        _data.ItemsData.ItemInfos.ForEach(x =>
        {
            AddButton(() =>
            {
                var cell = HexManager.UnitCurrentCell[_player.Color].cell.GetListNeighbours()
                    .First(hexCell => hexCell != null);

                x.Item.Spawn(cell, _itemsPrefab, ItemFabric.itemIcon[x.Item.Type]);
                scrollRect.SetActive(false);
            }, "Spawn " + x.Item.name, itemGrid.gameObject);
        });
        
        var playerGridGO = Instantiate(gridPrefab, grid.transform);
        _buttons.Add(playerGridGO);
        var playerGrid = playerGridGO.GetComponentInChildren<GridLayoutGroup>();
        playerGridGO.GetComponentInChildren<TMP_Text>().text = "Player";
        AddButton(() =>
        {
            _player.BaseView.OnHit.Invoke(_player.UnitData.maxHP);
            scrollRect.SetActive(false);
        }, "Kill Player", playerGrid.gameObject);
        
        _buttons.Add(AddButton(() => scrollRect.SetActive(false), "CLOSE", grid).gameObject);
    }

    private Button AddButton(Action onClickAction, string buttonText, GameObject parent)
    {
        var button = Instantiate(buttonPrefab, parent.transform);
        button.onClick.AddListener(onClickAction.Invoke);
        button.GetComponentInChildren<TMP_Text>().text = buttonText;
        return button;
    }

    public void OnPlayerDeath()
    {
        showButton.onClick.RemoveAllListeners();
        scrollRect.SetActive(false);
        _buttons.ForEach(Destroy);
    }
}