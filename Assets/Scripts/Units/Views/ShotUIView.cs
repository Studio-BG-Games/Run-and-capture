using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotUIView : MonoBehaviour
{
    [SerializeField] private GameObject shotOn;

    public void Switch()
    {
        shotOn.SetActive(!shotOn.activeSelf);
    }
}
