
using Chars;
using Data;
using UnityEngine;

namespace GameUI
{
    public class UIController
    {
        private readonly UIData _uiData;
        private PlayerControlView _playerControlView;

        public PlayerControlView PlayerControlView => _playerControlView;

        public UIController(UIData uiData)
        {
            _uiData = uiData;
        }

        public void Spawn()
        {
            var canvasGroup = new GameObject("CanvasGroup");
            canvasGroup.AddComponent<CanvasGroup>();
            _playerControlView = Object.Instantiate(_uiData.PlayerControlView, canvasGroup.transform);

           _uiData.ObjectsToSpawn.ForEach(x => Object.Instantiate(x, canvasGroup.transform));
        }
    }
}