using System;
using DG.Tweening;
using MainMenu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData _data;
        [SerializeField] private Image LevelImage;
        [SerializeField] private GameObject loadUI;
        private int index = 0;
        private TMP_Text _percentText;
        private bool _isLoadingLevel = false;
        private LevelData.Level _curLevel;
        private AsyncOperation _loadOpertion;

        private void Start()
        {
            loadUI.GetComponent<Image>().DOFade(0f, 0f);
            _curLevel = _data.Levels[0];
            SetLevelImage();
            _percentText = loadUI.GetComponentInChildren<TMP_Text>();
        }

        public void NextLevel()
        {
            if (index + 1 < _data.Levels.Count)
            {
                _curLevel = _data.Levels[++index];
                SetLevelImage();
            }
        }
        
        public void PrevLevel()
        {
            if (index - 1 >= 0)
            {
                _curLevel = _data.Levels[--index];
                SetLevelImage();
            }
        }

        private void SetLevelImage()
        {
            LevelImage.sprite = _curLevel.levelSprite;
        }

        public void LoadLevel()
        {
            loadUI.SetActive(true);
            loadUI.GetComponent<Image>().DOFade(1f, 0.3f).OnComplete( () =>
            {
                _isLoadingLevel = true;
                _loadOpertion = SceneManager.LoadSceneAsync(_curLevel.sceneName);
            }).SetEase(Ease.InQuad);
            
        }

        private void Update()
        {
            if (_isLoadingLevel && !_loadOpertion.isDone)
            {
                float progressValue = Mathf.Clamp01(_loadOpertion.progress / 0.9f);
               
                _percentText.text =  Mathf.Round(progressValue * 100) + " %";
            }
        }
    }
}