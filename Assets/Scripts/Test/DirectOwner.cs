using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectOwner : MonoBehaviour
{
    //[SerializeField] private GameObject _aiPrefab;
    public int count = 1;
    private CaptureController _crystallColor;

    [SerializeField] private ToweHealthController _parrantOwner; //= new ToweHealthController();
    [SerializeField] 
    private PlayerState _reseptOwver; // = new PlayerState();

    //public static TileInfo info = new TileInfo();


    private void Start() 
    {
        _parrantOwner.owner = GetComponent<ToweHealthController>().owner;
        _reseptOwver.ownerIndex = _parrantOwner.owner;
        //_reseptOwver.ownerIndex = _aiPrefab.GetComponent<PlayerState>().ownerIndex;
        //_parrantOwner = _aiPrefab.GetComponent<ToweHealthController>();
        //_crystallColor = _aiPrefab.GetComponent<CaptureController>();
        //_parrantOwner.owner = _reseptOwver.ownerIndex;
    }

    private void Update() 
    {
        _reseptOwver.ownerIndex = _parrantOwner.owner;
        //_crystallColor = new CaptureController();
        //info.tileOwnerIndex = _parrantOwner.owner;
        //_parrantOwner.owner = _reseptOwver.ownerIndex;
        //_reseptOwver.ownerIndex = _parrantOwner.owner;
    }

}
/*
    [SerializeField] private ToweHealthController _parrantOwner; //= new ToweHealthController();
    [SerializeField] private PlayerState _reseptOwver; //= new PlayerState();

    //public static TileInfo info = new TileInfo();


    private void Awake() 
    {
        _parrantOwner = GetComponent<ToweHealthController>();
    }

    private void FixedUpdate() 
    {
        //info.tileOwnerIndex = _parrantOwner.owner;
        _reseptOwver.ownerIndex = _parrantOwner.owner;
    }
    */