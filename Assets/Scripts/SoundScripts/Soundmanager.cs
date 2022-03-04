using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class Soundmanager // we write static to not accidently instantiate it
{
    public enum Sound
    {
        PlayerMove,
        PlayerRapidFireShoot,
        SingleShot,
        WeaponReload,
        EnemyHit,
        EnemyDie,
        MeleeWoosh,
        Buttonclick,
        StartGame,

    }

    static Dictionary<Sound, float> soundTimerDictionary;
    static GameObject oneShotGameObject;
    static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerMove] = 0f;
        soundTimerDictionary[Sound.EnemyHit] = 0f;
    }

    public static void PlaySound(Sound _sound)
    {
        if (CanPlaySound(_sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(_sound));
        }
    }

    // this is for 3D sound
    public static void PlaySound(Sound _sound, Vector3 _position)
    {
        if (CanPlaySound(_sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = _position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(_sound);
            audioSource.loop = false;
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }

    }


    static bool CanPlaySound(Sound _sound)
    {
        switch (_sound)
        {

            case Sound.PlayerMove:
                if (soundTimerDictionary.ContainsKey(_sound))
                {
                    float lastTimePlayed = soundTimerDictionary[_sound];
                    float playerMoveTimerMax = 0.4f; // we define a delay for the next playtime of the sound
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[_sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.PlayerRapidFireShoot:
                if (soundTimerDictionary.ContainsKey(_sound))
                {
                    float lastTimePlayed = soundTimerDictionary[_sound];
                    float playerMoveTimerMax = 1f; // we define a delay for the next playtime of the sound
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[_sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.EnemyHit:
                if (soundTimerDictionary.ContainsKey(_sound))
                {
                    float lastTimePlayed = soundTimerDictionary[_sound];
                    float enemyHiTimerMax = 5f;
                    if (lastTimePlayed + enemyHiTimerMax < Time.time)
                    {
                        soundTimerDictionary[_sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.EnemyDie:
                if (soundTimerDictionary.ContainsKey(_sound))
                {
                    float lastTimePlayed = soundTimerDictionary[_sound];
                    float enemyHiTimerMax = 5f;
                    if (lastTimePlayed + enemyHiTimerMax < Time.time)
                    {
                        soundTimerDictionary[_sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            default:
                return true;
                // break;
        }

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

    // this is an extension method
    // public static void AddButtonSounds(this Button_UI buttonUI)
    // {

    // }

}
