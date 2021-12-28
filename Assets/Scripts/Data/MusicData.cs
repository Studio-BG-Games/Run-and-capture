using System;
using MK.Toon;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "MusicData", menuName = "Data/MusicData", order = 0)]
    public class MusicData : ScriptableObject
    {
        [SerializeField] private AudioClip startMusic;
        [SerializeField] private AudioClip backMusic;
        [SerializeField] private SFXMusic sfxMusic;

        public AudioClip StartMusic => startMusic;
        public AudioClip BackMusic => backMusic;
        public SFXMusic SfxMusic => sfxMusic;
    }

    [Serializable]
        public struct SFXMusic
        {
            [SerializeField] private AudioClip _step;

            public AudioClip Step => _step;
        }
    }