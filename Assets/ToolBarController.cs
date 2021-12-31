using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarController : MonoBehaviour
{
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject cardsMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject equipmentMenu;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Button playMenuButton;
    [SerializeField] private Button cardsMenuButton;
    [SerializeField] private Button shopMenuButton;
    [SerializeField] private Button equipmentMenuButton;
    [SerializeField] private Button modeMenuButton;

    [SerializeField] private float duration;
    [SerializeField] private Ease ease;

    private void Awake()
    {
        playMenuButton.onClick.AddListener(() => ScrollToMenu(playMenu));
        cardsMenuButton.onClick.AddListener(() => ScrollToMenu(cardsMenu));
        shopMenuButton.onClick.AddListener(() => ScrollToMenu(shopMenu));
        equipmentMenuButton.onClick.AddListener(() => ScrollToMenu(equipmentMenu));
    }

    private void ScrollToMenu(GameObject menu)
    {
        mainMenu.transform.DOMove(-menu.transform.localPosition, duration).SetEase(ease);
  
    }
}
