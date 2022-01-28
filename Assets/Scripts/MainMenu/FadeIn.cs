using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private float duration;

    
    private void Start()
    {
        var back = GetComponent<Image>();
        back.DOFade(0, duration).OnComplete(() => gameObject.SetActive(false));
    }
}
