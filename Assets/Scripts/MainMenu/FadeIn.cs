using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class FadeIn : MonoBehaviour
    {
        [SerializeField] private float duration;

    
        private void Start()
        {
            var back = GetComponent<Image>();
            back.DOFade(0, duration).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
