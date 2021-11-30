using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictorVoices : MonoBehaviour
{
    public List<AudioClip> dictor;
    [SerializeField] private AudioSource aSoursce;

    private void OnEnable() {
        PlayRange();
    }
    private void OnDisable() {
        PlayRange();
    }

    void Update()
    {
        
        //PlayRange();
    }

    public void PlayRange()
    {
        int index = Random.Range(0, dictor.Count);
        aSoursce.clip = dictor[index];
        aSoursce.Play();

    }

}
