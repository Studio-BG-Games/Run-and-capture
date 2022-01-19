using UnityEngine;

namespace DefaultNamespace
{
    public static class SpawnHelper
    {
        public static GameObject Spawn(GameObject gameObject, Vector3 pos)
        {
           return Object.Instantiate(gameObject, pos, Quaternion.identity);
        }
        
        public static GameObject Spawn(GameObject gameObject, Vector3 pos, GameObject parrant)
        {
            return Object.Instantiate(gameObject, pos, Quaternion.identity, parrant.transform);
        }
        
        public static GameObject Spawn(GameObject gameObject,  GameObject parrant)
        {
            return Object.Instantiate(gameObject, parrant.transform);
        }
        public static void Destroy(GameObject obj){
            Object.Destroy(obj);
        }
    }
}