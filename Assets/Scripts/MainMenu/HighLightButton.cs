using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HighLightButton : MonoBehaviour
{
    [SerializeField] private GameObject highLighter;
    [SerializeField] private List<Button> _buttons;

    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;

    private void Start()
    {
        List<Transform> buttonTransforms = new List<Transform>();
        foreach (var button in _buttons)
        {
            buttonTransforms.Add(button.transform);
        }

        var i = 0;
        buttonTransforms.ForEach(x => { _buttons[i++].onClick.AddListener(() => Highlight(x)); });
        i = 0;
    }

    private void Highlight(Transform buttonTransform)
    {
        highLighter.transform
            .DOMove(new Vector3(buttonTransform.position.x, highLighter.transform.position.y, 0), _duration)
            .SetEase(_ease);
        
    }
}