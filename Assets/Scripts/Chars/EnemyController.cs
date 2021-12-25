using Controller;
using Data;
using Runtime.Controller;
using UnityEngine;

namespace Chars
{
    public class EnemyController : IFixedExecute, IExecute
    {
        private Enemy _enemy;
        private Camera _camera;

        public EnemyController(EnemyInfo enemyInfo, Enemy enemy)
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
            if (_enemy.EnemyView != null)
            {
                _enemy.EnemyView.BarCanvas.transform.LookAt(
                    _enemy.EnemyView.BarCanvas.transform.position + _camera.transform.rotation * Vector3.back,
                    _camera.transform.rotation * Vector3.up);
            }
        }
    }
}