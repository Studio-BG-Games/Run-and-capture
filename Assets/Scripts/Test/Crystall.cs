using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystall : MonoBehaviour
{
    public static DirectOwner _countOwners;
    public int _count = 0;

    [SerializeField] private List<DirectOwner> enemies ;
    private DirectOwner _cristallObject;

    private void Start() 
    {/*
        if( _countOwners == null)
        {
            _countOwners = GameObject.FindObjectOfType<DirectOwner>().GetComponent<DirectOwner>();
        }
        else
        {
            return;
        }
        _count = _countOwners.count;
        */
       // if(_countOwners == null)
       // _countOwners = ;
        //_csore._countOwners = GameObject.FindObjectOfType<DirectOwner>();
        //enemies;
        //enemies.Add(FindObjectOfType<DirectOwner>());
        //_cristallObject = GetComponent<DirectOwner>();

    }
    private void Update()
    {
        _count = FindObjectsOfType<DirectOwner>().Length;
        if(enemies.Count < _count)
            enemies.Add(FindObjectOfType<DirectOwner>());
        // for(int i = _count; i < enemies.Count; i++)
        // {
        //     enemies[i] = 
        // }
    }
}
