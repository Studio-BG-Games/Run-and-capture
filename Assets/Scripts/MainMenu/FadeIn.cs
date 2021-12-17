using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private float duration;
    private void OnEnable()
    {
        var back = GetComponent<Image>();
        back.DOFade(0, 5).OnComplete(() => gameObject.SetActive(false));
    }
}
