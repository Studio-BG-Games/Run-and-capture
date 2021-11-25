using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWithEffect : MonoBehaviour
{
    public GameObject prefVFX;
    void Start()
    {
        //Instantiate(prefVFX);
    }
    private void OnEnable() {

        Instantiate(prefVFX, gameObject.GetComponentInParent<Transform>());
    }
}
