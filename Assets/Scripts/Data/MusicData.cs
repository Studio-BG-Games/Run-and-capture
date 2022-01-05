using System;
using System.Collections.Generic;
using System.IO;
using MainMenu;
using MK.Toon;
using UnityEngine;
using AudioSettings = MainMenu.AudioSettings;

namespace Data
{
    [CreateAssetMenu(fileName = "MusicData", menuName = "Data/MusicData", order = 0)]
    public class MusicData : ScriptableObject
    {
        [SerializeField] private string _settingsDataPath;
        [SerializeField] private AudioClip startMusic;
        [SerializeField] private AudioClip backMusic;
        [SerializeField] private SFXMusic sfxMusic;

        public AudioClip StartMusic => startMusic;
        public AudioClip BackMusic => backMusic;
        public SFXMusic SfxMusic => sfxMusic;

        public AudioSettings Settings => JsonUtility.FromJson<AudioSettings>(File.ReadAllText(Application.persistentDataPath + "/" + _settingsDataPath));
    }

    [Serializable]
        public struct SFXMusic
        {
            [SerializeField] private List<AudioClip> steps;
            [SerializeField] private List<AudioClip> captures;
            [SerializeField] private AudioClip hardCapture;
            [SerializeField] private AudioClip death;
            public List<AudioClip> Captures => captures;
            public List<AudioClip> Step => steps;
            public AudioClip HardCapture => hardCapture;
            public AudioClip Death => death;
        }
    }