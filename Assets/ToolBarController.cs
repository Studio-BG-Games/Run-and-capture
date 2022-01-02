using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ToolBarController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private List<GameObject> content;

    [SerializeField] private List<Button> buttons;
    [SerializeField] private int startPage;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    
    private ScrollRect scrollRect;
    private int currentMenu;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        var i = 0;
        content.ForEach(con =>
        {
            buttons[i++].onClick.AddListener(delegate { ScrollToMenu(con); });
        });


        buttons[startPage].Select();
        buttons[startPage].onClick.Invoke();
        scrollRect.onValueChanged.AddListener(FixedScroll);
    }

    private void ScrollToMenu(GameObject menu)
    {
        Canvas.ForceUpdateCanvases();
        
        Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
        Vector2 childLocalPosition = menu.transform.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );

        scrollRect.content.DOLocalMove(result, duration).SetEase(ease);
        
    }

    private void FixedScroll(Vector2 vector2)
    {
        var step = 1f / (buttons.Count - 1);
        currentMenu = (int)Math.Round(vector2.x / step);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        buttons[currentMenu].Select();
        ScrollToMenu(content[currentMenu]);
    }
}