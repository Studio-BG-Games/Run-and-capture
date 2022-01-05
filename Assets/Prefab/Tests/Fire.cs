using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform barrel;
    public GameObject bulletPref;
    
    public GameObject[] ammo;
    public Stack<GameObject> ammoStack = new Stack<GameObject>();

    private int ammoAnount;

    private void Start() {
        //ammo.Push(bulletPref);
        for(int i = 0; i <=2; i++)
        {
            ammoStack.Push(ammo[i]);
            ammo[i].SetActive(false);
            Debug.Log($"Should print out :  {ammoStack.Peek()}");
            
        }
        // foreach (var item in ammoStack)
        // {
        //     item.SetActive(false);
        // }
        
        // if(ammo.Length <= 3)
        // {
        //     ammoStack.Pop().SetActive(false);
        //     Debug.Log($"Should print out :  {ammoStack.Peek().name}");
            
        // }
        ammoAnount = 0;
    } 

    private void Update() {
        if(Input.GetButtonDown("Fire1") && ammoAnount > 0)
        {
            var spawnBullet = Instantiate(bulletPref, barrel.localPosition, barrel.rotation);
            //spawnBullet.GetComponent<Rigidbody>().
            spawnBullet.transform.Translate(Vector3.forward * Time.deltaTime * 10f);
            ammoAnount -= 1;
            ammoStack.Pop().SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            ammoAnount = 3;
            for (int i = 0; i <= 2; i++)
            {
                ammoStack.Push(ammo[i]);
                ammo[i].SetActive(true);

            }
        }
    }
}
