using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Soundmanager // we write static to not accidently instantiate it
{
    public enum Sound
    {
        PlayerMove,
        PlayerRapidFireShoot,
        SingleShot,
        WeaponReload,
        EnemyHit,
        EnemyDie

    }


    public static void PlaySound(Sound _sound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(_sound));
    }

    static AudioClip GetAudioClip(Sound _sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == _sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + _sound + " not found");
        return null;
    }
}
