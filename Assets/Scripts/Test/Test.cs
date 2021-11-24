using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update() {
        if(Input.GetAxis("Vertical") != 0)
        {
            transform.Translate(Vector3.forward);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<BoxCollider>() != null)
        {
            Destroy(other.gameObject);
        }
    }
}
