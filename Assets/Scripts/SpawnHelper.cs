using UnityEngine;

namespace DefaultNamespace
{
    public static class SpawnHelper
    {
        public static GameObject Spawn(GameObject gameObject, Vector3 pos)
        {
           return Object.Instantiate(gameObject, pos, Quaternion.identity);
        }
    }
}