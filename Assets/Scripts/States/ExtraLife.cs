using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexFiled;

public class ExtraLife : MonoBehaviour
{
    [SerializeField] private GameObject canvasgroupe;
    [SerializeField] private AdsMob canvasPref;
    public static int lifeCount = 1;
    public int health = 1;

    private void Start() {
        
        canvasPref = FindObjectOfType<AdsMob>();
        canvasPref.gameObject.SetActive(false);
    }

    private void Update() {
        if(health <= 0)
        {
            lifeCount -= 1;   
            canvasPref.gameObject.SetActive(true);
            Time.timeScale = 0f;
            //canvasPref.ShowAd();
            //Instantiate(canvasPref);//.GetComponent<add>().ShowAd();      
            if(lifeCount < 0)
            {
                lifeCount = 0;
            }   
        }
        if(lifeCount > 0)
        {
            canvasPref.gameObject.SetActive(false);
            //Respawn();
            Time.timeScale = 1f;
        }

    }
    
    // public void Respawn()
    // {
    //     List<HexCell> cells = new List<HexCell>(FindObjectsOfType<HexCell>());
    //     // for (int i = 0; i < cells.Count; i++)
    //     // {

    //     // }
    //     foreach (var cell in cells)
    //     {
    //         if(cell.Color == UnitColor.GREY)
    //         {
    //             var randomCell = Random.Range(0, cells.Count);
    //             Vector3 respawnPosition = cells[randomCell].transform.position;
    //             GameObject player = FindObjectOfType<ExtraLife>().gameObject;
    //             player.transform.position = respawnPosition;
    //         }
    //     }
    // }

    // public void AddLIfe(int count)
    // {
        
    //     lifeCount = count;
    //     if(count < 1)
    //     {
    //         //add reward = new add();
    //         count++;            
    //     }

    // }

}
