using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEnemyTower : MonoBehaviour
{
    [SerializeField] private GameObject _towerPrefab;
    [SerializeField] private PlayerState _actualState;
    public List<ToweHealthController> controllList;
    public int count ;

    private void Update() 
    {

        if(gameObject.GetComponent<ToweHealthController>() != null)
        {
            List<GameObject> towers = new List<GameObject>();
            controllList = new List<ToweHealthController>();
            foreach(ToweHealthController controll in controllList)
            {
                if(controll.gameObject.name != gameObject.name)
                {
                    controllList.Add(controll);                
                }

                count = controllList.Count;                        
            }

        }


        /*
        if(gameObject.GetComponent<ToweHealthController>() != null)
            AddTower();
*/
    }
    public void AddTower()
    {
        if(  _towerPrefab.gameObject.name != gameObject.name && _towerPrefab.GetComponent<PlayerState>().ownerIndex != _actualState.ownerIndex)
        {
            _actualState.enemies.Add(FindObjectOfType<ToweHealthController>().GetComponent<PlayerState>());
        }

    }
}
