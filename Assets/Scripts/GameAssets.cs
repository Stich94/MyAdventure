using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    static GameAssets instance;

    public static GameAssets Instance
    {
        get { if (instance == null) instance = Instantiate(Resources.Load<GameAssets>("GameAssets")); return instance; }
    }

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public Soundmanager.Sound sound;
        public AudioClip audioClip;
    }
}
