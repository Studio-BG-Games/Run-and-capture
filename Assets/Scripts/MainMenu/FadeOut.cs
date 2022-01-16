using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class FadeOut : MonoBehaviour
    {
        [SerializeField] private float duration;
        private Image _target;

        private void Awake()
        {
            _target = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _target.DOFade(1, duration);
        }

        private void OnDisable()
        {
            var color = _target.color;
            color.a = 0f;
            _target.color = color;
        }
    }
}