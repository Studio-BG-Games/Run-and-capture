using Controller;
using Data;
using Runtime.Controller;
using UnityEngine;

namespace CameraControl
{
    public class CameraControl : IFixedExecute
    {
        private Camera _camera;
        private Transform _target;

        private Vector3 _offset;
        private float _smoothSpeed;

        public CameraControl(Camera camera, Transform target, CameraData cameraData)
        {
            _camera = camera;
            _target = target;
            _offset = cameraData.offset;
            _smoothSpeed = cameraData.smoothSpeed;
        }
        
        public void FixedExecute()
        {
            if (_target == null)
                return;
            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smothedPosition =
                Vector3.Lerp(_camera.transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
            _camera.transform.position = smothedPosition;
            _camera.transform.LookAt(_target);
        }
    }
}