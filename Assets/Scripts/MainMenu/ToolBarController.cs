using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ToolBarController : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;
    [SerializeField] private SimpleScrollSnap _scrollSnap;
    [SerializeField] private bool hasHightlighter;

    [SerializeField, ShowIf("hasHightlighter")]
    private GameObject highLighter;

    [SerializeField, ShowIf("highLighter")] private Ease ease;
    [SerializeField, ShowIf("highLighter")] private float duration;

    private void Start()
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            var i1 = i;
            buttons[i].onClick.AddListener(() =>
            {
                _scrollSnap.GoToPanel(i1);
                buttons[i1].Select();
                if (hasHightlighter)
                {
                    Highlight(buttons[i1].transform);
                    _scrollSnap.onPanelChanged.AddListener(() =>
                    {
                        Highlight(buttons[_scrollSnap.CurrentPanel].transform); 
                        buttons[_scrollSnap.CurrentPanel].Select();
                    });
                }
            });
        }
    }

    private void Highlight(Transform buttonTransform)
    {
        highLighter.transform
            .DOMove(new Vector3(buttonTransform.position.x, highLighter.transform.position.y, 0), duration)
            .SetEase(ease);
    }
}