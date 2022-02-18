using UnityEngine;

namespace Controller
{
  
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Data.Data data;
        private Controllers _controllers;

        private void Awake()
        {
            _controllers = new Controllers();
            new GameInit(_controllers, data);
           
            _controllers.Awake();
            
        }

        private void Start()
        {
            _controllers.Init();
        }

        private void Update()
        {
            _controllers.Execute();
        }

        private void FixedUpdate()
        {
            _controllers.FixedExecute();
        }

        private void LateUpdate()
        {
            _controllers.LateExecute();
        }

        private void OnDestroy()
        {
            _controllers.Cleanup();
            
        }
    }
}