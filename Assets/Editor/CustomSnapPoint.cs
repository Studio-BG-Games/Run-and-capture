using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSnapPoint : MonoBehaviour
{
   private void OnDrawGizmos()
   {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, radius: 0.05f);
   }
}
