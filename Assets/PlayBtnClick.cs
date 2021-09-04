using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayBtnClick : MonoBehaviour
{
    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();

        btn.onClick.AddListener(BtnTask);
    }

    private void BtnTask()
    {
        SceneLoader.LoadNextScene();
    }
}
