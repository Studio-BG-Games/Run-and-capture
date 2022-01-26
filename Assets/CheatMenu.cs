using System;
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
    private GameObject ItemsPrefab;

    public void SetPlayerNData(Unit player, Data.Data data)
    {
        _player = player;
        ItemsPrefab = new GameObject("CheatedItems");
        
        showButton.onClick.AddListener(() => scrollRect.SetActive(!scrollRect.activeSelf));
        _data = data;
        AddAllButtons();
        scrollRect.SetActive(false);
        
    }

    private void AddAllButtons()
    {
        
        var itemGridGo = Instantiate(gridPrefab, grid.transform);
        var itemGrid = itemGridGo.GetComponentInChildren<GridLayoutGroup>();
        itemGridGo.GetComponentInChildren<TMP_Text>().text = "Items";
        _data.ItemsData.ItemInfos.ForEach(x =>
        {
            AddButton(() =>
            {
                var cell = HexManager.UnitCurrentCell[_player.Color].cell.GetListNeighbours()
                    .First(hexCell => hexCell != null);

                x.Item.Spawn(cell, ItemsPrefab, ItemFabric.itemIcon[x.Item.Type]);
                scrollRect.SetActive(false);
            }, "Spawn " + x.Item.name, itemGrid.gameObject);
        });
        
        var playerGridGO = Instantiate(gridPrefab, grid.transform);
        var playerGrid = playerGridGO.GetComponentInChildren<GridLayoutGroup>();
        playerGridGO.GetComponentInChildren<TMP_Text>().text = "Player";
        AddButton(() =>
        {
            _player.UnitView.OnHit.Invoke(_player.Data.maxHP);
            scrollRect.SetActive(false);
        }, "Kill Player", playerGrid.gameObject);
        AddButton(() => scrollRect.SetActive(false), "CLOSE", grid);
    }

    private Button AddButton(Action onClickAction, string buttonText, GameObject parent)
    {
        var button = Instantiate(buttonPrefab, parent.transform);
        button.onClick.AddListener(onClickAction.Invoke);
        button.GetComponentInChildren<TMP_Text>().text = buttonText;
        return button;
    }
}