
using Chars;
using Data;
using UnityEngine;

namespace GameUI
{
    public class UIController
    {
        private readonly UIData _uiData;

        public PlayerControlView PlayerControlView { get; private set; }

        public PlayerInventoryView PlayerInventoryView { get; private set; }

        public AdsMob AdsMob { get; private set; }

        public CheatMenu CheatMenu { get; private set; }
        public WariorSpawnView WariorSpawnView { get; private set; }

        public UIController(UIData uiData)
        {
            _uiData = uiData;
        }

        public void Spawn()
        {
            var canvasGroup = Object.Instantiate(_uiData.Canvas);

            PlayerControlView = Object.Instantiate(_uiData.PlayerControlView, canvasGroup.transform);
            PlayerInventoryView = Object.Instantiate(_uiData.InventoryView, canvasGroup.transform);
            _uiData.ObjectsToSpawn.ForEach(x => Object.Instantiate(x, canvasGroup.transform));
            CheatMenu = Object.Instantiate(_uiData.CheatMenu, canvasGroup.transform);
            WariorSpawnView = Object.Instantiate(_uiData.WariorSpawnView, canvasGroup.transform);
            AdsMob = Object.Instantiate(_uiData.AdsMob, canvasGroup.transform);

        }
    }
}