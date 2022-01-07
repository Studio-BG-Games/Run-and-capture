
using Chars;
using Data;
using UnityEngine;

namespace GameUI
{
    public class UIController
    {
        private readonly UIData _uiData;
        private PlayerControlView _playerControlView;
        private PlayerInventoryView _inventoryView;
        private AdsMob _add;

        public PlayerControlView PlayerControlView => _playerControlView;
        public PlayerInventoryView PlayerInventoryView => _inventoryView;
        public AdsMob Add => _add;

        public UIController(UIData uiData)
        {
            _uiData = uiData;
        }

        public void Spawn()
        {
            var canvasGroup = new GameObject("CanvasGroup");
            canvasGroup.AddComponent<CanvasGroup>();
            _playerControlView = Object.Instantiate(_uiData.PlayerControlView, canvasGroup.transform);
            _inventoryView = Object.Instantiate(_uiData.InventoryView, canvasGroup.transform);
            _add = Object.Instantiate(_uiData.Add, canvasGroup.transform);
            //_add.enabled = false;
            
            _uiData.ObjectsToSpawn.ForEach(x => Object.Instantiate(x, canvasGroup.transform));
        }
    }
}