using Controller;
using Data;
using Runtime.Controller;
using Units;
using UnityEngine;

namespace Chars
{
    public class EnemyController : IFixedExecute, IExecute
    {
        private Unit _enemy;
        private Camera _camera;

        public EnemyController(UnitInfo enemyInfo, Unit enemy)
        {
            _enemy = enemy;
            _camera = Camera.main;
            
        }
        
        public void FixedExecute()
        {
            //throw new System.NotImplementedException();
        }

        public void Execute()
        {
            if (_enemy.UnitView != null)
            {
                _enemy.UnitView.BarCanvas.transform.LookAt(
                    _enemy.UnitView.BarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                    _camera.transform.rotation * Vector3.up);
            }
        }
    }
}