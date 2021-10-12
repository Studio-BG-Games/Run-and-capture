using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoadedAssets.Joystick_Pack.Scripts.Joysticks
{
    public class OpacityJoystick : Joystick
    {
        [SerializeField] private float _idleStateOpacity = 1f;
        [SerializeField] private float _activeStateOpacity = 1f;

        private Image _backgroundImage;
        private Image _handleImage;

        private bool _hasBackgroundImage;
        private bool _hasHandleImage;


        protected override void Start()
        {
            _backgroundImage = background.GetComponent<Image>();
            _handleImage = handle.GetComponent<Image>();

            _hasBackgroundImage = _backgroundImage != null;
            _hasHandleImage = _handleImage != null;
            SetOpacity(_idleStateOpacity);
            base.Start();
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            SetOpacity(_activeStateOpacity);
            base.OnPointerDown(eventData);
        }


        public override void OnPointerUp(PointerEventData eventData)
        {
            SetOpacity(_idleStateOpacity);
            base.OnPointerUp(eventData);
        }


        private void SetOpacity(float opacity)
        {
            if (_hasBackgroundImage)
            {
                var backgroundColor = _backgroundImage.color;
                backgroundColor.a = opacity;
                _backgroundImage.color = backgroundColor;
            }

            if (_hasHandleImage)
            {
                var handleColor = _handleImage.color;
                handleColor.a = opacity;
                _handleImage.color = handleColor;
            }
        }
    }
}