using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Count : MonoBehaviour
{
    public static int lifesValue = 0;

    //[SerializeField] private Extralife lifeBefor;
    Text amount;

    // Start is called before the first frame update
    void Start()
    {
        amount = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        amount.text = lifesValue.ToString();
    }
}
