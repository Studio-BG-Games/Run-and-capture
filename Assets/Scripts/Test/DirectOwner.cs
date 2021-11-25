using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectOwner : MonoBehaviour
{
    //[SerializeField] private GameObject _aiPrefab;
    public int count = 1;
    private CaptureController _crystallColor;

    [SerializeField] private ToweHealthController _parrantOwner; 
    [SerializeField] 
    private PlayerState _reseptOwver; 




    private void Start() 
    {
        _parrantOwner.owner = GetComponent<ToweHealthController>().owner;
        _reseptOwver.ownerIndex = _parrantOwner.owner;

    }

    private void Update() 
    {
        _reseptOwver.ownerIndex = _parrantOwner.owner;
    }

}
