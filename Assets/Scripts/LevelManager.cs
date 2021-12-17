using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData _data;
        [SerializeField] private Image LevelImage;
        private int index = 0;
        private LevelData.Level _curLevel;

        private void Start()
        {
            _curLevel = _data.Levels[0];
            SetLevelImage();
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
            SceneManager.LoadScene(_curLevel.sceneName);
        }

    }
}