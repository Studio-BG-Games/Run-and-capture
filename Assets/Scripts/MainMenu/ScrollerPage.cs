using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollerPage : MonoBehaviour, IEndDragHandler
    {
        [SerializeField] private GameObject content;

        [SerializeField] private float duration;
        [SerializeField] private Ease ease;
        private ScrollRect _scrollRect;
        private int currentMenu;
        private Vector2 startMotion;

        public event Action<int> OnLevelChanged;

        public void Init()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _scrollRect.onValueChanged.AddListener(FixedScroll);
            ScrollToMenu(content.transform.GetChild(0).gameObject);
        }
        
        private void ScrollToMenu(GameObject menu)
        {
            Canvas.ForceUpdateCanvases();
        
            Vector2 viewportLocalPosition = _scrollRect.viewport.localPosition;
            Vector2 childLocalPosition = menu.transform.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );

            _scrollRect.content.DOLocalMove(result, duration).SetEase(ease);
        
        }
        
        private void FixedScroll(Vector2 vector2)
        {
            var step = 1f / (content.transform.childCount - 1);
            currentMenu = (int)Math.Round(vector2.x / step);
            
        }

       
        
        public void OnEndDrag(PointerEventData eventData)
        {
            ScrollToMenu(content.transform.GetChild(currentMenu).gameObject);
            OnLevelChanged?.Invoke(currentMenu);
        }
    }
}
