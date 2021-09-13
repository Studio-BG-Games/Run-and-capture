using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepsSound : MonoBehaviour
{
    private AudioController _controller;
    private void Start()
    {
        _controller = transform.parent.GetComponent<AudioController>();
    }

    public void Step()
    {
        _controller.PlayJumpSound();
    }
}
