using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData _data;
        [SerializeField] private LevelView LevelImage;
        [SerializeField] private GameObject loadUI;
        [SerializeField] private SimpleScrollSnap _levelScroll;
        private int index = 0;
        private TMP_Text _percentText;
        private bool _isLoadingLevel = false;
        private LevelData.Level _curLevel;
        private AsyncOperation _loadOpertion;

        private void Start()
        {
            loadUI.GetComponent<Image>().DOFade(0f, 0f);
            _curLevel = _data.Levels[0];
            var i = 0;
            _data.Levels.ForEach(level =>
            {
                
                var lev = Object.Instantiate(LevelImage);
                lev.LevelImage.sprite = level.levelSprite;
                _levelScroll.Add(lev.gameObject, i++);
               
               
            });

            _levelScroll.onPanelChanged.AddListener(() => SelectLevel(_levelScroll.CurrentPanel));
            _percentText = loadUI.GetComponentInChildren<TMP_Text>();
        }


        private void SelectLevel(int curentMenu)
        {
            _curLevel = _data.Levels[curentMenu];
        }
        
        public void LoadLevel()
        {
            loadUI.SetActive(true);
            loadUI.GetComponent<Image>().DOFade(1f, 0.3f).OnComplete(() =>
            {
                _loadOpertion = SceneManager.LoadSceneAsync(_curLevel.sceneName);
                _isLoadingLevel = true;
            }).SetEase(Ease.InQuad);
            Debug.Log(_curLevel.sceneName);
        }

        private void Update()
        {
            if (_isLoadingLevel && !_loadOpertion.isDone)
            {
                float progressValue = Mathf.Clamp01(_loadOpertion.progress / 0.9f);

                _percentText.text = Mathf.Round(progressValue * 100) + " %";
            }
        }
    }
}